using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.GridBrushBase;

public class PlayerShooting : MonoBehaviour
{
    //private GameObject[] ammoUIObjects;
    private GameObject reloadUISlider;

    private bool canFire = true;

    private Vector2 aimInput = Vector2.right;
    private Vector3 shotRotation = new Vector3(0,0,0);
    private bool _isUsingMovementToAim = false;

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
    private PlayerMovement _playerMovement;
    private GameObject _bulletUIObject;
    private BulletUIManager _bulletUIManager;

    private bool _hasCompanion = false;
    private CompanionManager _companionManager;

    public void InitialiseComponent(ref PlayerData playerData, ref PlayerDebugData debugData, ref PlayerMovement playerMovement, ref ObjectPoolManager poolManager, ref GameObject dUICanvas)
    {
        _playerData = playerData;
        _debugData = debugData;

        _playerMovement = playerMovement;

        currentAmmo = _playerData.maxAmmo;

        _poolManager = poolManager;

        _bulletUIObject = Instantiate(_playerData.ammoUIPrefab, dUICanvas.transform);
        _bulletUIManager = _bulletUIObject.GetComponent<BulletUIManager>();

        reloadUISlider = Instantiate(_playerData.reloadUISlider, dUICanvas.transform);
        reloadUISlider.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + new Vector3(aimInput.x, aimInput.y, 0.0f) * 10.0f);

        if (interrupted) { return; }

        if (takeShot)
        {
            if (Time.time - lastShotTime >= _playerData.fireRate) // Waits until the can shoot (works from buffer)
            {
                takeShot = false;

                if (_isUsingMovementToAim)
                {
                    aimInput = _playerMovement.GetMovementDirection();
                }

                if (chargedAmmo == 0)
                {
                    Fire(aimInput, 1); // Regular shot
                    return;
                }

                // Charged shot
                FireChargedShots(aimInput);
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

    public void UpdateCompanionData(bool isAdding, ref CompanionManager companionManager)
    {
        _hasCompanion = isAdding;
        _companionManager = companionManager;
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

    public void SetMouseAimInput(InputAction.CallbackContext context)
    {
        _isUsingMovementToAim = false;

        Vector2 inputValue = context.ReadValue<Vector2>();
        if(inputValue == new Vector2(transform.position.x, transform.position.y)) { return; }

        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(inputValue.x, inputValue.y, Camera.main.transform.position.z));
        Vector2 aimDirection = new Vector2(worldPoint.x - transform.position.x, worldPoint.y - transform.position.y);

        aimInput = aimDirection.normalized;
    }

    public void SetControllerAimInput(InputAction.CallbackContext context)
    {
        _isUsingMovementToAim = false;

        Vector2 inputValue = context.ReadValue<Vector2>();
        if (inputValue == Vector2.zero) { return; }

        aimInput = inputValue;
    }

    public void SetAimToMovement(InputAction.CallbackContext context)
    {
        _isUsingMovementToAim = true;
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

        //for (int i = 0; i < currentAmmo; i++)
        //{
        //    ammoUIObjects[i].SetActive(false);
        //}
        _bulletUIManager.DeactivateAll();

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

    private void Fire(Vector2 fireDirection, int ammoUsed, float fireMultiplier = 1.0f)
    {
        float rotz = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;

        GameObject projectile = _poolManager.GetFreeObject(_playerData.baseProjectileType.name);
        projectile.transform.position = transform.position + new Vector3(fireDirection.normalized.x, fireDirection.normalized.y, 0)*2.0f;
        projectile.transform.rotation = Quaternion.Euler(0, 0, rotz);
        projectile.GetComponent<ProjectileBehaviour>().InitialiseComponent(fireDirection.normalized,
            _playerData.baseProjectileSpeed,
            ref _poolManager,
            _playerData.baseProjectileType.name,
            gameObject,
            fireMultiplier);

        if (_hasCompanion)
        {
            projectile.GetComponent<ProjectileBehaviour>().AddCompanionTargetting(ref _companionManager);
        }

        lastShotTime = Time.time;

        currentAmmo -= ammoUsed;

        _bulletUIManager.UpdateLoadedBullets(currentAmmo);

        if (currentAmmo <= 0 && !reloading) // Trying to shoot with no ammo
        {
            StartCoroutine(ReloadAmmo());
            return;
        }
    }

    private IEnumerator ReloadAmmo()
    {
        reloading = true;

        _bulletUIManager.StartReloadAnim();

        //foreach (GameObject obj in ammoUIObjects)
        //{
        //    obj.GetComponent<Image>().color = Color.white;
        //}

        float startTime = Time.time;

        //reloadUISlider.SetActive(true);
        //reloadUISlider.GetComponent<Slider>().value = 0;
        while (Time.time - startTime <= _playerData.reloadTime)
        {
        //    reloadUISlider.GetComponent<Slider>().value = (Time.time - startTime) / _playerData.reloadTime * 100;
            yield return null;
        }
        //reloadUISlider.SetActive(false);

        currentAmmo = _playerData.maxAmmo;

        reloading = false;

        //foreach (GameObject obj in ammoUIObjects)
        //{
        //    obj.SetActive(true);
        //}

        _bulletUIManager.EndReloadAnim();

        _bulletUIManager.UpdateLoadedBullets(currentAmmo);
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
        //ammoUIObjects[currentAmmo - chargedAmmo].GetComponent<Image>().color = Color.blue;
        _bulletUIManager.UpdateChargedBulletsUI(chargedAmmo);
    }

    private void FireChargedShots(Vector2 direction)
    {
        float localDamageMultiplier = 1;

        for (int i = 0; i < chargedAmmo; i++)
        {
            localDamageMultiplier *= _playerData.damageMultiplier;
        }

        Fire(direction, chargedAmmo, localDamageMultiplier);

        ReleaseChargedShots();
    }

    private void ReleaseChargedShots()
    {
        //foreach (GameObject obj in ammoUIObjects)
        //{
        //    obj.GetComponent<Image>().color = Color.white;
        //}

        chargedAmmo = 0;

        _bulletUIManager.UpdateChargedBulletsUI(chargedAmmo);
    }
    #endregion
}
