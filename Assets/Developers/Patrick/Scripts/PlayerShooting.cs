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

    private ObjectPoolManager poolManager;

    private float fireRate;
    private float fireTime;
    private float startShot = 0.0f;
    private float chargeShotTime = 0.0f;
    private bool startCharging = false;
    private float timeForChargedShot = 1.0f;
    private bool takeShot = false;
    private float chargeShotIntervals = 0.0f;
    private float lastShotTime = 0.0f;

    private float fireInputBuffer;

    private int maxAmmo;
    private int chargedAmmo = 0;
    private int currentAmmo;
    private float reloadTime = 1.0f;

    private bool reloading = false;
    private bool firingChargedShot = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InitialiseComponent(GameObject projectile, float dProjectileSpeed, float dFireRate, int dMaxAmmo, bool dCanDropWeapon, ref ObjectPoolManager dPoolManager)
    {
        projectilePrefab = projectile;
        projectileSpeed = dProjectileSpeed;
        fireRate = dFireRate;
        maxAmmo = dMaxAmmo;
        canDropWeapon = dCanDropWeapon;
        poolManager = dPoolManager;

        currentAmmo = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)));

        if (takeShot)
        {
            if(Time.time - lastShotTime >= fireRate) // Waits until the can shoot (works from buffer)
            {
                takeShot = false;
                if (chargedAmmo == 0)
                {
                    Fire(); // Regular shot
                    return;
                }

                // Charged shot
                StartCoroutine(FireChargedShots());
            }
        }
        else if (startCharging)
        {
            if(Time.time - timeForChargedShot < fireRate)
            {
                if(currentAmmo - chargedAmmo <= 0)
                {
                    startCharging = false; // Reached max charged Ammo
                }

                ChargeAmmo();
                timeForChargedShot = Time.time;
            }
        }
    }

    #region Input

    public void PlayerFireInput(InputAction.CallbackContext context)
    {
        if (reloading)
        {
            return;
        }

        if(currentAmmo <= 0)
        {
            StartCoroutine(ReloadAmmo());
        }

        startShot = Time.time;
        timeForChargedShot = Time.time;

        startCharging = true;
    }

    public void PlayerStopFireInput(InputAction.CallbackContext context)
    {
        if (!CanFire()) {  return; }

        // Signals we want to fire
        takeShot = true;
        startCharging = false;
    }

    #endregion

    private bool CanFire()
    {
        return (Time.time - lastShotTime < fireRate - fireInputBuffer); // Is the fire time within the buffer or past the fire rate
    }

    private void Fire()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));

        Vector3 rotation = mouseWorldPos - transform.position;
        float rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        GameObject projectile = poolManager.GetFreeObject(projectilePrefab.name);
        projectile.transform.position = transform.position;
        projectile.transform.rotation = Quaternion.Euler(0, 0, rotz);
        projectile.GetComponent<ProjectileBehaviour>().InitialiseComponent((new Vector2(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).x, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).y) - new Vector2(transform.position.x, transform.position.y)).normalized,
            projectileSpeed,
            ref poolManager,
            projectilePrefab.name,
            gameObject);
        fireTime = Time.time;
    }

    private void ChargeAmmo()
    {
        chargedAmmo++;
    }

    private IEnumerator FireChargedShots()
    {
        firingChargedShot = true;

        float startTime = Time.time;

        for (int i = 0; i < chargedAmmo; i++)
        {
            Fire();

            while (Time.time - startTime <= reloadTime)
            {
                yield return null;
            }
        }

        firingChargedShot = false;
    }

    private IEnumerator ReloadAmmo()
    {
        reloading = true;

        float startTime = Time.time;
        while(Time.time - startTime <= reloadTime)
        {
            yield return null;
        }
        
        currentAmmo = maxAmmo;

        reloading = false;
    }

    #region Weapon Drop
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
    #endregion
}
