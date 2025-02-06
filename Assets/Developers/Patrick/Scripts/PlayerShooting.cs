using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    public float projectileSpeed = 10;
    public GameObject projectilePrefab;

    private bool canDropWeapon = false;
    public bool canFire = true;
    private bool fireInput = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InitialiseComponent(GameObject projectile, float dProjectileSpeed, bool dCanDropWeapon)
    {
        projectilePrefab = projectile;
        projectileSpeed = dProjectileSpeed;
        canDropWeapon = dCanDropWeapon;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)));

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));

        if (fireInput)
        {
            Vector3 rotation = mouseWorldPos - transform.position;
            float rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

            ProjectileBehaviour projectile = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, rotz)).GetComponent<ProjectileBehaviour>();
            projectile.Instantiate((new Vector2(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).x, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).y) - new Vector2(transform.position.x, transform.position.y)).normalized, projectileSpeed);

            fireInput = false;
        }
    }

    public void PlayerFireInput(InputAction.CallbackContext context)
    {
        if (canFire)
        {
            fireInput = true;
        }
    }

    public void DisableFire()
    {
        if (canDropWeapon)
        {
            canFire = false;
        }
    }

    public void EnableFire()
    {
        if (canDropWeapon)
        {
            canFire = true;
        }
    }
}
