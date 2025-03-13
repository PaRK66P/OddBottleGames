using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    private GameObject[] ammoUIObjects;
    private GameObject reloadUISlider;

    private bool canFire = true;

    private ObjectPoolManager _poolManager;

    private bool startCharging = false;
    private float timeForChargedShot = 1.0f;
    private bool takeShot = false;
    private float lastShotTime = 0.0f;

    private int currentAmmo = 0;
    private int chargedAmmo = 0;

    private bool reloading = false;
    private bool firingChargedShot = false;
    private bool interrupted = false;
    private bool charging = false;

    private PlayerData _playerData;
    private PlayerDebugData _debugData;

    public void InitialiseComponent(ref PlayerData playerData, ref PlayerDebugData debugData, ref ObjectPoolManager poolManager, ref GameObject dUICanvas)
    {
        _playerData = playerData;
        _debugData = debugData;

        currentAmmo = _playerData.maxAmmo;

        _poolManager = poolManager;

        ammoUIObjects = new GameObject[_playerData.maxAmmo];

        float offset = (_playerData.maxAmmo / 2) * 0.3f - 0.2f;
        for (int i = 0; i < _playerData.maxAmmo; i++)
        {
            ammoUIObjects[i] = Instantiate(_playerData.ammoUIObject, dUICanvas.transform);

            ammoUIObjects[i].GetComponent<RectTransform>().position = transform.position;
            ammoUIObjects[i].GetComponent<RectTransform>().Translate(new Vector3(-offset + (i * 0.3f), 1.45f, 0));
        }

        reloadUISlider = Instantiate(_playerData.reloadUISlider, dUICanvas.transform);
        reloadUISlider.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)));

        if(interrupted) { return; }

        if (takeShot)
        {
            if(Time.time - lastShotTime >= _playerData.fireRate) // Waits until the can shoot (works from buffer)
            {
                takeShot = false;
                if (chargedAmmo == 0)
                {
                    Fire((new Vector2(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).x, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).y) - new Vector2(transform.position.x, transform.position.y)),
                        Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)) - transform.position,
                        1); // Regular shot
                    return;
                }

                // Charged shot
                FireChargedShots((new Vector2(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).x, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)).y) - new Vector2(transform.position.x, transform.position.y)),
                    Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)) - transform.position);
            }
        }
        else if (startCharging)
        {
            if(Time.time - timeForChargedShot >= Mathf.Lerp(_playerData.maxTimeToChargeShot, _playerData.minTimeToChargeShot, Mathf.Min(chargedAmmo / (_playerData.shotsTillFullCharge - 1.0f), 1.0f)))
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

    public void PlayerChargeInput(InputAction.CallbackContext context)
    {
        if (reloading) // Currently reloading or can't fire
        {
            return;
        }

        // Start charging a shot
        timeForChargedShot = Time.time;

        startCharging = true;
        interrupted = false;
        charging = true;
    }

    public void PlayerStopChargeInput(InputAction.CallbackContext context)
    {
        startCharging = false;

        // Check if firing here as we can charge immediately but not fire immediately
        if (!CanFire() || !charging || chargedAmmo <= 0) 
        {
            charging = false;
            if (!firingChargedShot)
            {
                ReleaseChargedShots();
            }
            return; 
        }

        charging = false;

        // Signals we want to fire
        takeShot = true;
    }

    public void PlayerFireInput(InputAction.CallbackContext context)
    {
        if(charging || !CanFire()) { return; }
        interrupted = false;
        takeShot = true;
    }

    public void PlayerReloadAction(InputAction.CallbackContext context)
    {
        if(charging || reloading || currentAmmo == _playerData.maxAmmo) { return; }

        for (int i = 0; i < currentAmmo; i++)
        {
            ammoUIObjects[i].SetActive(false);
        }

        StartCoroutine(ReloadAmmo());
    }

    #endregion

    #region Firing
    private bool CanFire()
    {
        // Conditions to fire
        /*
         * Fire rate with buffer consideration
         * Not reloading
         */
        return (Time.time - lastShotTime >= _playerData.fireRate - _debugData.firingInputBuffer && !reloading);
    }

    private void Fire(Vector2 fireDirection, Vector3 rotation, int ammoUsed, float fireMultiplier = 1.0f)
    {
        float rotz = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        GameObject projectile = _poolManager.GetFreeObject(_playerData.baseProjectileType.name);
        projectile.transform.position = transform.position + new Vector3(fireDirection.normalized.x, fireDirection.normalized.y, 0)*2.0f;
        projectile.transform.rotation = Quaternion.Euler(0, 0, rotz);
        projectile.GetComponent<ProjectileBehaviour>().InitialiseComponent(fireDirection.normalized,
            _playerData.baseProjectileSpeed,
            ref _poolManager,
            _playerData.baseProjectileType.name,
            gameObject,
            fireMultiplier);
        lastShotTime = Time.time;

        currentAmmo -= ammoUsed;

        for (int i = currentAmmo; i < currentAmmo + ammoUsed; i++)
        {
            ammoUIObjects[i].SetActive(false);
        }

        if (currentAmmo <= 0 && !reloading) // Trying to shoot with no ammo
        {
            StartCoroutine(ReloadAmmo());
            return;
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
        while (Time.time - startTime <= _playerData.reloadTime)
        {
            reloadUISlider.GetComponent<Slider>().value = (Time.time - startTime) / _playerData.reloadTime * 100;
            yield return null;
        }
        reloadUISlider.SetActive(false);

        currentAmmo = _playerData.maxAmmo;

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

    private void FireChargedShots(Vector2 direction, Vector3 rotation)
    {
        float localDamageMultiplier = 1;

        for (int i = 0; i < chargedAmmo; i++)
        {
            localDamageMultiplier *= _playerData.damageMultiplier;
        }

        Fire(direction, rotation, chargedAmmo, localDamageMultiplier);

        ReleaseChargedShots();
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

    //#region Weapon Drop
    //public void DisableFire()
    //{
    //    if (_debugData.canDropWeapon)
    //    {
    //        canFire = false;
    //    }
    //}

    //public void EnableFire()
    //{
    //    if (_debugData.canDropWeapon)
    //    {
    //        canFire = true;
    //    }
    //}
    //#endregion
}
