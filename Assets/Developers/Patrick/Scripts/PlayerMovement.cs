using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    private Vector2 movementInput;
    public float speed = 5;
    public float dashDistance;
    public int dashChargesMaxNumber = 3;
    public float dashChargeTime = 3;

    private GameObject[] dashChargesUIObject;

    public LayerMask damageLayers;

    public bool dash = false;
    private float dashTimer = 0.0f;
    private int dashChargesNumber = 0;
    private float dashChargeTimer = 0.0f;
    private Vector2 movementDirection = Vector2.up;
    private Vector2 dashStart = Vector2.zero;
    private Vector2 dashDirection = Vector2.zero;
    private float dashTime = 0.2f;
    private float dashCooldown = 0.2f;
    private float dashInputBuffer = 0.0f;
    private float lastDashTime = -10.0f;
    private float lastDashInputTime = -10.0f;

    public bool dashTowardsMouse = false;

    private Vector2 knockbackForce = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void InitialiseComponent(float dSpeed, float dDashTime, float dDashDistance, float dDashCooldown, float dDashInputBuffer, LayerMask dDamageLayers, bool dashingTowardsMouse, int dDashCharges, float dDashChargeTime, ref GameObject dUICanvas, GameObject dDashChargeUIObject)
    {
        speed = dSpeed;
        dashTime = dDashTime;
        dashDistance = dDashDistance;
        dashCooldown = dDashCooldown;
        dashInputBuffer = dDashInputBuffer;
        damageLayers = dDamageLayers;
        dashTowardsMouse = dashingTowardsMouse;
        dashChargesNumber = dDashCharges;
        dashChargesMaxNumber = dDashCharges;
        dashChargeTime = dDashChargeTime;
        dashChargesUIObject = new GameObject[dashChargesNumber];

        for (int i = 0; i < dashChargesNumber; i++)
        {
            dashChargesUIObject[i] = Instantiate(dDashChargeUIObject, dUICanvas.transform);

            dashChargesUIObject[i].GetComponent<RectTransform>().Translate(Vector3.down * 100 * (i + 1));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(dashChargesNumber < dashChargesMaxNumber)
        {
            dashChargeTimer += Time.deltaTime;
        }
        if(dashChargeTimer >= dashChargeTime)
        {
            dashChargeTimer = 0.0f;
            if(dashChargesNumber < dashChargesMaxNumber)
            {
                dashChargesNumber++;
            }
        }

        switch (dashChargesNumber)
        {
            case 0:
                for (int i = 0; i < 3; i++)
                {
                    dashChargesUIObject[i].SetActive(false);
                }
                break;
            case 1:
                dashChargesUIObject[0].SetActive(true);
                for (int i = 1; i < 3; i++)
                {
                    dashChargesUIObject[i].SetActive(false);
                }
                break;
            case 2:
                dashChargesUIObject[2].SetActive(false);
                for (int i = 0; i < 2; i++)
                {
                    dashChargesUIObject[i].SetActive(true);
                }
                break;
            case 3:
                for (int i = 0; i < 3; i++)
                {
                    dashChargesUIObject[i].SetActive(true);
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
            if (Time.time - lastDashTime >= dashCooldown && dashChargesNumber > 0) // Off cooldown
            {
                StartCoroutine(DashColor());
                dashTimer += Time.fixedDeltaTime;

                rb.excludeLayers = damageLayers;

                rb.MovePosition(Vector2.Lerp(dashStart, dashStart + dashDirection * dashDistance, Mathf.Min(dashTimer / dashTime, 1.0f)));

                if (dashTimer >= dashTime)
                {
                    rb.excludeLayers = 0;

                    dash = false;

                    lastDashTime = Time.time;

                    dashChargesNumber--;
                }

                return;
            }
            else if (Time.time - lastDashInputTime > dashInputBuffer) // Buffer has been exceeded
            {
                dash = false;
            }

        }

        rb.velocity = movementInput * speed;
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
            dashDirection = dashTowardsMouse ? new Vector2(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).x - transform.position.x, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).y - transform.position.y).normalized
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
        yield return new WaitForSeconds(dashTime);
        spriteRenderer.color = Color.white;
    }
}
