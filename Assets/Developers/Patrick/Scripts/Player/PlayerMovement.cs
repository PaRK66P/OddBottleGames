using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    private Vector2 movementInput;

    private GameObject[] dashChargesUIObjects;

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

    private PlayerData _playerData;
    private PlayerDebugData _debugData;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void InitialiseComponent(ref PlayerData playerData, ref PlayerDebugData debugData, ref GameObject UICanvas)
    {
        _playerData = playerData;
        _debugData = debugData;

        dashChargesNumber = _playerData.numberOfDashCharges;
        maxDashCharges = 3;

        dashChargesUIObjects = new GameObject[maxDashCharges];

        for (int i = 0; i < maxDashCharges; i++)
        {
            dashChargesUIObjects[i] = Instantiate(_playerData.dashChargeUIObject, UICanvas.transform);

            dashChargesUIObjects[i].GetComponent<RectTransform>().Translate(Vector3.down * 100 * (i + 1));
            dashChargesUIObjects[i].transform.SetParent(UICanvas.transform.Find("PlayerUI"), true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(dashChargesNumber < maxDashCharges)
        {
            dashChargeTimer += Time.deltaTime;
        }
        if(dashChargeTimer >= _playerData.dashRechargeTime)
        {
            dashChargeTimer = 0.0f;

            if (dashChargesNumber < maxDashCharges)
            {
                dashChargesNumber++;
            }
        }

        switch (dashChargesNumber)
        {
            case 0:
                for (int i = 0; i < 3; i++)
                {
                    dashChargesUIObjects[i].SetActive(false);
                }
                break;
            case 1:
                dashChargesUIObjects[0].SetActive(true);
                for (int i = 1; i < 3; i++)
                {
                    dashChargesUIObjects[i].SetActive(false);
                }
                break;
            case 2:
                dashChargesUIObjects[2].SetActive(false);
                for (int i = 0; i < 2; i++)
                {
                    dashChargesUIObjects[i].SetActive(true);
                }
                break;
            case 3:
                for (int i = 0; i < 3; i++)
                {
                    dashChargesUIObjects[i].SetActive(true);
                }
                break;
            default:
                break;
        }
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

                rb.excludeLayers = _playerData.damageLayers;

                rb.MovePosition(Vector2.Lerp(dashStart, dashStart + dashDirection * _playerData.dashDistance, Mathf.Min(dashTimer / _playerData.dashTime, 1.0f)));

                if (dashTimer >= _playerData.dashTime)
                {
                    rb.excludeLayers = 0;

                    dash = false;

                    lastDashTime = Time.time;

                    dashChargesNumber--;
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

        // Can implement acceleration in the future but not sure if necessary for this style of game

        Vector2 speedDiff = targetSpeed - rb.velocity;

        // Normally apply acceleration but we want instant so just use speed
        Vector2 movement =  speedDiff * _playerData.speed;

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
            //Debug.Log(dashTowardsMouse);
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
        spriteRenderer.color = Color.blue;
        yield return new WaitForSeconds(_playerData.dashTime);
        spriteRenderer.color = Color.white;
    }
}
