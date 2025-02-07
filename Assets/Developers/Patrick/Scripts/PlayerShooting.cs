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

    private ObjectPoolManager poolManager;

    private float fireRate;
    private float fireTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InitialiseComponent(GameObject projectile, float dProjectileSpeed, float dFireRate, bool dCanDropWeapon, ref ObjectPoolManager dPoolManager)
    {
        projectilePrefab = projectile;
        projectileSpeed = dProjectileSpeed;
        fireRate = dFireRate;
        canDropWeapon = dCanDropWeapon;
        poolManager = dPoolManager;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)));

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));

        if (fireInput && Time.time - fireTime >= fireRate)
        {
            Vector3 rotation = mouseWorldPos - transform.position;
            float rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

            GameObject projectile = poolManager.GetFreeObject(projectilePrefab.name);
            projectile.transform.position = transform.position;
            projectile.transform.rotation = Quaternion.Euler(0, 0, rotz);
            projectile.GetComponent<ProjectileBehaviour>().InitialiseComponent((new Vector2(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).x, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).y) - new Vector2(transform.position.x, transform.position.y)).normalized, 
                projectileSpeed,
                ref poolManager,
                projectilePrefab.name);
            fireTime = Time.time;
        }
    }

    public void PlayerFireInput(InputAction.CallbackContext context)
    {
        fireInput = true;
    }

    public void PlayerStopFireInput(InputAction.CallbackContext context)
    {
        fireInput = false;
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
