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
    public float dashDistance = 3;

    public LayerMask damageLayers;
    public LayerMask empty;

    public bool canFire = true;

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

            Debug.Log(mouseWorldPos);

            ProjectileBehaviour projectile = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, rotz)).GetComponent<ProjectileBehaviour>();
            projectile.Instantiate((new Vector2(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).x, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).y) - new Vector2(transform.position.x, transform.position.y)).normalized, projectileSpeed);
        }
        
    }

    private void FixedUpdate()
    {
        rb.velocity = movementInput * speed;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));
        Vector2 direction = new Vector2(mouseWorldPos.x - transform.position.x, mouseWorldPos.y - transform.position.y).normalized;

        if (Input.GetButtonDown("Fire1"))
        {
            rb.excludeLayers = damageLayers;
            rb.MovePosition(new Vector2(transform.position.x + direction.x * dashDistance, transform.position.y + direction.y * dashDistance));
            rb.excludeLayers = empty;
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
