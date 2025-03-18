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

    public LayerMask damageLayers;

    public bool dash = false;
    private float dashTimer = 0.0f;
    private Vector2 movementDirection = Vector2.up;
    private Vector2 dashStart = Vector2.zero;
    private Vector2 dashDirection = Vector2.zero;
    private float dashTime = 0.2f;
    private float dashCooldown = 0.2f;
    private float dashInputBuffer = 0.0f;
    private float lastDashTime = -10.0f;
    private float lastDashInputTime = -10.0f;

    private bool dashTowardsMouse = false;

    private Vector2 knockbackForce = Vector2.zero;

    private SoundManager soundManager;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void InitialiseComponent(float dSpeed, float dDashTime, float dDashDistance, float dDashCooldown, float dDashInputBuffer, LayerMask dDamageLayers, bool dashingTowardsMouse, SoundManager dSoundManager)
    {
        speed = dSpeed;
        dashTime = dDashTime;
        dashDistance = dDashDistance;
        dashCooldown = dDashCooldown;
        dashInputBuffer = dDashInputBuffer;
        damageLayers = dDamageLayers;
        dashTowardsMouse = dashingTowardsMouse;
        soundManager = dSoundManager;
    }

    // Update is called once per frame
    void Update()
    {

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

        if (dash)
        {
            if(Time.time - lastDashTime >= dashCooldown) // Off cooldown
            {
                dashTimer += Time.fixedDeltaTime;

                rb.excludeLayers = damageLayers;
                soundManager.PlayPDash(); //Bugged; need to play once, not every second
                rb.MovePosition(Vector2.Lerp(dashStart, dashStart + dashDirection * dashDistance, Mathf.Min(dashTimer / dashTime, 1.0f)));

                if (dashTimer >= dashTime)
                {
                    rb.excludeLayers = 0;

                    dash = false;

                    lastDashTime = Time.time;
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
            dash = true;
            dashTimer = 0.0f;
            dashStart = new Vector2(transform.position.x, transform.position.y);
            dashDirection = dashTowardsMouse ? new Vector2(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).x - transform.position.x, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).y - transform.position.y).normalized
                : movementDirection;
        }
    }

    public void KnockbackPlayer(Vector2 direction, float scale)
    {
        knockbackForce = direction * scale;
    }
}
