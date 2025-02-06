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
    public LayerMask empty;

    public bool dash = false;
    private float dashTimer = 0.0f;
    private Vector2 movementDirection = Vector2.up;
    private Vector2 dashStart = Vector2.zero;
    private Vector2 dashDirection = Vector2.zero;
    private float dashTime = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        empty = new LayerMask();
        rb = GetComponent<Rigidbody2D>();
    }

    public void InitialiseComponent(float dSpeed, float dDashTime, float dDashDistance, LayerMask dDamageLayers)
    {
        speed = dSpeed;
        dashTime = dDashTime;
        dashDistance = dDashDistance;
        damageLayers = dDamageLayers;
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    private void FixedUpdate()
    {
        if (dash)
        {

            dashTimer += Time.fixedDeltaTime;

            rb.excludeLayers = damageLayers;

            rb.MovePosition(Vector2.Lerp(dashStart, dashStart + dashDirection * dashDistance, Mathf.Min(dashTimer / dashTime, 1.0f)));

            if(dashTimer >= dashTime)
            {
                rb.excludeLayers = empty;

                dash = false;
            }
        }
        else
        {
            rb.velocity = movementInput * speed;
            if(rb.velocity != Vector2.zero)
            {
                movementDirection = rb.velocity.normalized;
            }
        }
    }

    public void SetMovementInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void PlayerDashInput(InputAction.CallbackContext context)
    {
        Debug.Log("Dash");

        dash = true;
        dashTimer = 0.0f;
        dashStart = new Vector2(transform.position.x, transform.position.y);
        dashDirection = movementDirection;
    }
}
