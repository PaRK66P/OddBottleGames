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
    public float projectileSpeed = 10;
    public GameObject projectilePrefab;
    public float dashDistance;

    public LayerMask damageLayers;
    public LayerMask empty;

    public bool canFire = true;

    public bool dash = false;
    private float dashTimer = 0.0f;
    private Vector2 dashStart = Vector2.zero;
    private Vector2 dashDirection = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)));

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));
        Vector2 direction = new Vector2(mouseWorldPos.x - transform.position.x, mouseWorldPos.y - transform.position.y).normalized;

        if (Input.GetMouseButtonDown(0) && canFire)
        {
            Vector3 rotation = mouseWorldPos - transform.position;
            float rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

            //Debug.Log(mouseWorldPos);

            ProjectileBehaviour projectile = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, rotz)).GetComponent<ProjectileBehaviour>();
            projectile.Instantiate((new Vector2(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).x, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).y) - new Vector2(transform.position.x, transform.position.y)).normalized, projectileSpeed);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            dash = true;
            dashTimer = 0.0f;
            dashStart = new Vector2(transform.position.x, transform.position.y);
            dashDirection = direction;
        }

    }

    private void FixedUpdate()
    {


        if (dash)
        {
            dashTimer += Time.fixedDeltaTime;

            rb.excludeLayers = damageLayers;

            rb.MovePosition(Vector2.Lerp(dashStart, dashStart + dashDirection * dashDistance, Mathf.Min(dashTimer / 0.5f, 1.0f)));

            if(dashTimer > 0.2f)
            {
                rb.excludeLayers = empty;

                dash = false;
            }
            
        }
        else
        {
            rb.velocity = movementInput * speed;
        }
    }

    public void SetMovementInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void DisableFire()
    {
        canFire = false;
    }

    public void EnableFire()
    {
        canFire = true;
    }
}
