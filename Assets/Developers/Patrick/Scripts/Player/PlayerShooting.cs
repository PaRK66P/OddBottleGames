using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    private float projectileSpeed = 10;
    private GameObject projectilePrefab;
    private GameObject[] ammoUIObjects;
    private GameObject reloadUISlider;

    private bool canDropWeapon = false;
    private bool canFire = true;

    private ObjectPoolManager poolManager;

    private float fireRate;
    private bool startCharging = false;
    private float timeForChargedShot = 1.0f;
    private float maxChargeUpShotTime = 0.2f;
    private float minChargeUpShotTime = 0.5f;
    private int shotsTillFullChargeUp = 4;
    private bool takeShot = false;
    private float chargeShotIntervals = 0.0f;
    private float lastShotTime = 0.0f;
    private float damageMultiplier = 1.5f;

    private float fireInputBuffer;

    private int maxAmmo;
    private int chargedAmmo = 0;
    private int currentAmmo;
    private float reloadTime = 1.0f;

    private bool reloading = false;
    private bool firingChargedShot = false;
    private bool interrupted = false;
    private bool charging = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InitialiseComponent(GameObject dAmmoUIObject, float dFireRate, float dMaxChargeUpShotTime, float dMinChargeUpShotTime, 
        int dShotsTillFullChargeUp, float dChargeShotIntervals, int dMaxAmmo, float dReloadTime, GameObject dBaseProjectileType, 
        float dBaseProjectileSpeed, float dFiringInputBuffer, bool dCanDropWeapon,ref ObjectPoolManager dPoolManager, ref GameObject dUICanvas,
        GameObject dReloadUISlider, float dDamageMultiplier)
    {

        fireRate = dFireRate;

        maxChargeUpShotTime = dMaxChargeUpShotTime;
        minChargeUpShotTime = dMinChargeUpShotTime;
        shotsTillFullChargeUp = dShotsTillFullChargeUp;
        chargeShotIntervals = dChargeShotIntervals;

        damageMultiplier = dDamageMultiplier;

        maxAmmo = dMaxAmmo;
        currentAmmo = maxAmmo;

        projectilePrefab = dBaseProjectileType;
        projectileSpeed = dBaseProjectileSpeed;
        
        fireInputBuffer = dFiringInputBuffer;
        canDropWeapon = dCanDropWeapon;

        poolManager = dPoolManager;

        ammoUIObjects = new GameObject[maxAmmo];

        float offset = (maxAmmo / 2) * 0.3f - 0.2f;
        for (int i = 0; i < maxAmmo; i++)
        {
            ammoUIObjects[i] = Instantiate(dAmmoUIObject, dUICanvas.transform);

            ammoUIObjects[i].GetComponent<RectTransform>().position = transform.position;
            ammoUIObjects[i].GetComponent<RectTransform>().Translate(new Vector3(-offset + (i * 0.3f), 1.45f, 0));
        }

        reloadUISlider = Instantiate(dReloadUISlider, dUICanvas.transform);
        reloadUISlider.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)));

        if(interrupted) { return; }

        if (takeShot)
        {
            if(Time.time - lastShotTime >= fireRate) // Waits until the can shoot (works from buffer)
            {
                takeShot = false;
                if (chargedAmmo == 0)
                {
                    Fire((new Vector2(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).x, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).y) - new Vector2(transform.position.x, transform.position.y)),
                        Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)) - transform.position); // Regular shot
                    return;
                }

                // Charged shot
                StartCoroutine(FireChargedShots((new Vector2(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).x, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).y) - new Vector2(transform.position.x, transform.position.y)),
                    Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)) - transform.position));
            }
        }
        else if (startCharging)
        {
            if(Time.time - timeForChargedShot >= Mathf.Lerp(maxChargeUpShotTime, minChargeUpShotTime, Mathf.Min(chargedAmmo / (shotsTillFullChargeUp - 1.0f), 1.0f)))
            {
                if(currentAmmo - chargedAmmo <= 0)
                {
                    startCharging = false; // Reached max charged Ammo
                    return;
                }

                ChargeAmmo();
                timeForChargedShot = Time.time;
            }
        }
    }

    #region Input

    public void PlayerFireInput(InputAction.CallbackContext context)
    {
        if (reloading || !canFire) // Currently reloading or can't fire
        {
            return;
        }

        // Start charging a shot
        timeForChargedShot = Time.time;

        startCharging = true;
        interrupted = false;
        charging = true;
    }

    public void PlayerStopFireInput(InputAction.CallbackContext context)
    {
        startCharging = false;

        // Check if firing here as we can charge immediately but not fire immediately
        if (!CanFire()) 
        {
            if (!firingChargedShot)
            {
                ReleaseChargedShots();
            }
            return; 
        }

        // Signals we want to fire
        takeShot = true;
        charging = false;
    }

    public void PlayerReloadAction(InputAction.CallbackContext context)
    {
        if(charging || reloading || currentAmmo == maxAmmo) { return; }

        StartCoroutine(ReloadAmmo());
    }

    #endregion

    #region Firing
    private bool CanFire()
    {
        // Conditions to fire
        /*
         * Fire rate with buffer consideration
         * Not currently firing
         * Not reloading
         * Currently charging
         */
        return (Time.time - lastShotTime >= fireRate - fireInputBuffer && !reloading && charging && !firingChargedShot);
    }

    private void Fire(Vector2 fireDirection, Vector3 rotation, float fireMultiplier = 1.0f)
    {
        float rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        GameObject projectile = poolManager.GetFreeObject(projectilePrefab.name);
        projectile.transform.position = transform.position;
        projectile.transform.rotation = Quaternion.Euler(0, 0, rotz);
        projectile.GetComponent<ProjectileBehaviour>().InitialiseComponent(fireDirection.normalized,
            projectileSpeed,
            ref poolManager,
            projectilePrefab.name,
            gameObject,
            fireMultiplier);
        lastShotTime = Time.time;

        if (fireMultiplier == 1)
        {
            currentAmmo--;
            ammoUIObjects[currentAmmo].SetActive(false);

            if (currentAmmo <= 0 && !reloading) // Trying to shoot with no ammo
            {
                StartCoroutine(ReloadAmmo());
                return;
            }
        }
    }

    private IEnumerator ReloadAmmo()
    {
        reloading = true;

        foreach (GameObject obj in ammoUIObjects)
        {
            obj.GetComponent<Image>().color = Color.white;
        }

        float startTime = Time.time;

        reloadUISlider.SetActive(true);
        reloadUISlider.GetComponent<Slider>().value = 0;
        while (Time.time - startTime <= reloadTime)
        {
            reloadUISlider.GetComponent<Slider>().value = (Time.time - startTime) / reloadTime * 100;
            yield return null;
        }
        reloadUISlider.SetActive(false);

        currentAmmo = maxAmmo;

        reloading = false;

        foreach (GameObject obj in ammoUIObjects)
        {
            obj.SetActive(true);
        }
    }

    public void InterruptFiring()
    {
        interrupted = true;
        startCharging = false;
        charging = false;
        ReleaseChargedShots();
    }

    #endregion

    #region Charge Firing
    private void ChargeAmmo()
    {
        chargedAmmo++;
        ammoUIObjects[currentAmmo - chargedAmmo].GetComponent<Image>().color = Color.blue;
    }

    private IEnumerator FireChargedShots(Vector2 direction, Vector3 rotation)
    {
        firingChargedShot = true;

        float startTime;

        float localDamageMultiplier = 1;

        for (int i = 0; i < chargedAmmo; i++)
        {
            if (interrupted)
            {
                break;
            }

            startTime = Time.time;

            //Fire(direction, rotation);
            localDamageMultiplier *= damageMultiplier;

            while (Time.time - startTime <= chargeShotIntervals && !interrupted)
            {
                yield return null;
            }
        }

        Fire(direction, rotation, localDamageMultiplier);

        for (int j = 0; j < chargedAmmo;j++)
        {
            currentAmmo--;
            ammoUIObjects[currentAmmo].SetActive(false);
        }

        if (currentAmmo <= 0 && !reloading) // Trying to shoot with no ammo
        {
            StartCoroutine(ReloadAmmo());
        }

        ReleaseChargedShots();

        firingChargedShot = false;
    }

    private void ReleaseChargedShots()
    {
        foreach (GameObject obj in ammoUIObjects)
        {
            obj.GetComponent<Image>().color = Color.white;
        }

        chargedAmmo = 0;
    }
    #endregion

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
