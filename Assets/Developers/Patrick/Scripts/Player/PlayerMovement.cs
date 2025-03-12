using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerManager _playerManager;
    private Rigidbody2D rb;
    private GameObject UICanvas;
    private HealthBarScript healthBarScript;

    private Vector2 movementInput;

    private GameObject[] dashChargesUIObjects;
    private GameObject[] dashRechargesUIObjects;

    public bool dash = false;
    private float dashTimer = 0.0f;
    private int dashChargesNumber = 0;
    private int maxDashCharges = 0;
    private float dashChargeTimer = 0.0f;
    private Vector2 movementDirection = Vector2.up;
    private Vector2 dashStart = Vector2.zero;
    private Vector2 dashDirection = Vector2.zero;
    private float lastDashTime = -10.0f;
    private float lastDashInputTime = -10.0f;

    private Vector2 knockbackForce = Vector2.zero;

    private bool evolved = false;

    private PlayerData _playerData;
    private PlayerDebugData _debugData;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();        
    }

    public void InitialiseComponent(ref PlayerManager playerManager, ref PlayerData playerData, ref PlayerDebugData debugData, ref GameObject dUICanvas, ref HealthBarScript dHealthbarScript)
    {
        _playerManager = playerManager;

        UICanvas = dUICanvas;

        _playerData = playerData;
        _debugData = debugData;

        healthBarScript = dHealthbarScript;

        dashChargesNumber = 1;
        maxDashCharges = 1;

        //dashRechargesUIObjects = new GameObject[maxDashCharges];
        //dashChargesUIObjects = new GameObject[maxDashCharges];

        //for (int i = 0; i < maxDashCharges; i++)
        //{
        //    dashRechargesUIObjects[i] = Instantiate(_playerData.dashRechargeUIObject, UICanvas.transform);
        //    dashRechargesUIObjects[i].GetComponent<RectTransform>().Translate(Vector3.down * 100 * (i + 1));
        //    dashRechargesUIObjects[i].transform.SetParent(UICanvas.transform, true);
        //    //dashRechargesUIObjects[i].GetComponent<RectTransform>().localScale = new Vector3(0, 0, 1);

        //    dashChargesUIObjects[i] = Instantiate(_playerData.dashChargeUIObject, UICanvas.transform);
        //    dashChargesUIObjects[i].GetComponent<RectTransform>().Translate(Vector3.down * 100 * (i + 1));
        //    dashChargesUIObjects[i].transform.SetParent(UICanvas.transform, true);
        //}
        healthBarScript.setDashUI(dashChargesNumber);
    }

    // Update is called once per frame
    void Update()
    {
        if(dashChargesNumber < maxDashCharges)
        {
            dashChargeTimer += Time.deltaTime;

            //dashRechargesUIObjects[dashChargesNumber].GetComponent<RectTransform>().localScale = new Vector3(dashChargeTimer / _playerData.dashRechargeTime, dashChargeTimer / _playerData.dashRechargeTime, 1);
        }
        if(dashChargeTimer >= _playerData.dashRechargeTime)
        {
            dashChargeTimer = 0.0f;

            if (dashChargesNumber < maxDashCharges)
            {
                //dashRechargesUIObjects[dashChargesNumber].GetComponent<RectTransform>().localScale = new Vector3(0, 0, 1);
                dashChargesNumber++;
                healthBarScript.setDashUI(dashChargesNumber);
            }
        }

        //for(int i = 0; i < maxDashCharges; ++i)
        //{
        //    if(dashChargesNumber > i)
        //    {
        //        dashChargesUIObjects[i].SetActive(true);
        //    }
        //    else
        //    {
        //        dashChargesUIObjects[i].SetActive(false);
        //    }
        //    if(dashChargesNumber == i)
        //    {
        //        dashRechargesUIObjects[i].SetActive(true);
        //    }
        //    else
        //    {
        //        dashRechargesUIObjects[i].SetActive(false);
        //    }
        //}
    }

    private void FixedUpdate()
    {
        if (movementInput != Vector2.zero)
        {
            // Do we need this?
            movementDirection = movementInput.normalized;
        }

        if (knockbackForce != Vector2.zero)
        {
            dash = false;

            rb.AddForce(knockbackForce, ForceMode2D.Impulse);
            knockbackForce = Vector2.zero;
            return;
        }

        //if (dash)
        //{
        //    if(Time.time - lastDashTime >= dashCooldown) // Off cooldown
        //    {
        //        dashTimer += Time.fixedDeltaTime;

        //        rb.excludeLayers = damageLayers;

        //        rb.MovePosition(Vector2.Lerp(dashStart, dashStart + dashDirection * dashDistance, Mathf.Min(dashTimer / dashTime, 1.0f)));

        //        if (dashTimer >= dashTime)
        //        {
        //            rb.excludeLayers = 0;

        //            dash = false;

        //            lastDashTime = Time.time;
        //        }
        //        return;
        //    }
        //    else if (Time.time - lastDashInputTime > dashInputBuffer) // Buffer has been exceeded
        //    {
        //        dash = false;
        //    }

        //}

        if (dash)
        {
            if (Time.time - lastDashTime >= _playerData.dashCooldown && dashChargesNumber > 0) // Off cooldown
            {
                StartCoroutine(DashColor());
                dashTimer += Time.fixedDeltaTime;

                _playerManager.SetDashInvulnerability(true);
                //GetComponentInChildren<EvolveDashDamage>().UpdateCollisionLayer(_playerData.damageLayers);
                // Update evolve dash collision layers if active

                rb.MovePosition(Vector2.Lerp(dashStart, dashStart + dashDirection * (evolved ? _playerData.dashDistance + _playerData.evolvedDashExtraDistance : _playerData.dashDistance), Mathf.Min(dashTimer / _playerData.dashTime, 1.0f)));

                if (dashTimer >= _playerData.dashTime)
                {
                    _playerManager.SetDashInvulnerability(false);

                    dash = false;

                    lastDashTime = Time.time;

                    dashChargesNumber--;
                    healthBarScript.setDashUI(dashChargesNumber);
                }

                return;
            }
            else if (Time.time - lastDashInputTime > _debugData.dashInputBuffer) // Buffer has been exceeded
            {
                dash = false;
            }

        }

        Movement();
    }

    private void Movement()
    {
        // Get the speed the player wants to move at
        Vector2 targetSpeed = movementInput * _playerData.speed;

        // Calculate acceleration type
        float accelerationRate = (targetSpeed.sqrMagnitude > 0.0001f) ? _playerData.accelerationRate : _playerData.decelerationRate;

        Vector2 speedDiff = targetSpeed - rb.velocity;

        Vector2 movement =  speedDiff * accelerationRate;

        rb.AddForce(movement, ForceMode2D.Force);
    }

    public void SetMovementInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void PlayerDashInput(InputAction.CallbackContext context)
    {

        lastDashInputTime = Time.time;
        if (!dash)
        {
            dash = true;
            dashTimer = 0.0f;
            dashStart = new Vector2(transform.position.x, transform.position.y);
            dashDirection = _debugData.dashTowardsMouse ? new Vector2(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).x - transform.position.x, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).y - transform.position.y).normalized
                : movementDirection;

            //StartCoroutine(DashColor());
        }
    }

    public void KnockbackPlayer(Vector2 direction, float scale)
    {
        knockbackForce = direction * scale;
    }

    IEnumerator DashColor()
    {
        SpriteRenderer spriteRenderer = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.color = Color.cyan;
        yield return new WaitForSeconds(_playerData.dashTime);
        if (spriteRenderer.color == Color.cyan)
        {
            spriteRenderer.color = Color.white;
        }
    }

    public void EvolveDash()
    {
        for (int i = 0; i < maxDashCharges; i++)
        {
            Destroy(dashRechargesUIObjects[i]);
            Destroy(dashChargesUIObjects[i]);
        }

        maxDashCharges = _playerData.numberOfDashCharges;
        dashChargesNumber = maxDashCharges;

        dashRechargesUIObjects = new GameObject[maxDashCharges];
        dashChargesUIObjects = new GameObject[maxDashCharges];

        for (int i = 0; i < maxDashCharges; i++)
        {
            dashRechargesUIObjects[i] = Instantiate(_playerData.dashRechargeUIObject, UICanvas.transform);
            dashRechargesUIObjects[i].GetComponent<RectTransform>().Translate(Vector3.down * 100 * (i + 1));
            dashRechargesUIObjects[i].transform.SetParent(UICanvas.transform, true);
            //dashRechargesUIObjects[i].GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);

            dashChargesUIObjects[i] = Instantiate(_playerData.dashChargeUIObject, UICanvas.transform);
            dashChargesUIObjects[i].GetComponent<RectTransform>().Translate(Vector3.down * 100 * (i + 1));
            dashChargesUIObjects[i].transform.SetParent(UICanvas.transform, true);
        }

        evolved = true;
    }

    public void RechargeDashes(int n = 3)
    {
        if (maxDashCharges != _playerData.numberOfDashCharges)
        {
            if(n > 1)
            {
                n = 1;
            }
        }

        dashChargesNumber += n;

        if(dashChargesNumber > _playerData.numberOfDashCharges)
        {
            dashChargesNumber = _playerData.numberOfDashCharges;
            dashChargeTimer = 0.0f;
        }
    }
}
