using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    private Vector2 movementInput;

    public bool dash = false;
    private float dashTimer = 0.0f;
    private Vector2 movementDirection = Vector2.up;
    private Vector2 dashStart = Vector2.zero;
    private Vector2 dashDirection = Vector2.zero;
    private float lastDashTime = -10.0f;
    private float lastDashInputTime = -10.0f;

    private Vector2 knockbackForce = Vector2.zero;

    private PlayerData playerData;
    private PlayerDebugData debugData;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void InitialiseComponent(PlayerData chosenPlayerData, PlayerDebugData chosenDebugData)
    {
        playerData = chosenPlayerData;
        debugData = chosenDebugData;
    }

    // Update is called once per frame
    void Update()
    {

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

        if (dash)
        {
            if(Time.time - lastDashTime >= playerData.dashCooldown) // Off cooldown
            {
                StartCoroutine(DashColor());
                dashTimer += Time.fixedDeltaTime;

                rb.excludeLayers = playerData.damageLayers;

                rb.MovePosition(Vector2.Lerp(dashStart, dashStart + dashDirection * playerData.dashDistance, Mathf.Min(dashTimer / playerData.dashTime, 1.0f)));

                if (dashTimer >= playerData.dashTime)
                {
                    rb.excludeLayers = 0;

                    dash = false;

                    lastDashTime = Time.time;
                }
                return;
            }
            else if (Time.time - lastDashInputTime > debugData.dashInputBuffer) // Buffer has been exceeded
            {
                dash = false;
            }
            
        }

        Movement();
    }

    private void Movement()
    {
        // Get the speed the player wants to move at
        Vector2 targetSpeed = movementInput * playerData.speed;

        // Can implement acceleration in the future but not sure if necessary for this style of game

        Vector2 speedDiff = targetSpeed - rb.velocity;

        // Normally apply acceleration but we want instant so just use speed
        Vector2 movement =  speedDiff * playerData.speed;

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
            dashDirection = debugData.dashTowardsMouse ? new Vector2(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).x - transform.position.x, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).y - transform.position.y).normalized
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
        yield return new WaitForSeconds(playerData.dashTime);
        spriteRenderer.color = Color.white;
    }
}
