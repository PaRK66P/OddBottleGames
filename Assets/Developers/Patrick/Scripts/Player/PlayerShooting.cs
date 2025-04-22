using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    // Objects
    private ObjectPoolManager _objectPoolManager;
    private TimeManager _timeManager;
    private GameObject _bulletUIObject;
    private CompanionManager _companionManager;
    private SoundManager _soundManager;
    private PlayerData _playerData;
    private PlayerDebugData _debugData;

    // Components
    private PlayerMovement _playerMovement;
    private PlayerAnimationHandler _playerAnimationHandler;
    private BulletUIManager _bulletUIManager;

    // Values
    private Vector2 _aimInput = Vector2.right;
    private Vector2 _lastAimInput = Vector2.right;
    private bool _isUsingMovementToAim = false;

    private bool _hasStartedCharging = false;
    private float _timeForChargedShot = 1.0f;
    private bool _canTakeShot = false;
    private float _lastShotTime = 0.0f;

    private int _currentAmmo = 0;
    private int _chargedAmmo = 0;

    private bool _isReloading = false;
    private bool _isFiringChargedShot = false;
    private bool _isInterrupted = false;
    private bool _isCharging = false;
    private bool _isCocking = false;

    private bool _hasCompanion = false;

    public void InitialiseComponent(ref PlayerData playerData, ref PlayerDebugData debugData, ref PlayerMovement playerMovement, ref PlayerAnimationHandler playerAnimations, ref ObjectPoolManager poolManager, ref TimeManager timeManager, ref SoundManager soundManager, ref GameObject canvasUI)
    {
        _playerData = playerData;
        _debugData = debugData;

        _playerMovement = playerMovement;
        _playerAnimationHandler = playerAnimations;

        _currentAmmo = _playerData.MaxAmmo;

        _objectPoolManager = poolManager;
        _timeManager = timeManager;

        _soundManager = soundManager;

        _bulletUIObject = Instantiate(_playerData.AmmoUIPrefab, canvasUI.transform);
        _bulletUIManager = _bulletUIObject.GetComponent<BulletUIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Cocking sound
        if (_isCocking)
        {
            if(Time.time - _lastShotTime >= _playerData.FireRate)
            {
                _isCocking = false;
                if (!_isCharging)
                {
                    _soundManager.PlayPGunCock();
                }
            }
        }

        if (_isInterrupted) { return; }

        if (_canTakeShot)
        {
            // Wants to shoot
            if (Time.time - _lastShotTime >= _playerData.FireRate) // Waits until the can shoot (works from buffer)
            {
                _canTakeShot = false;

                if (_chargedAmmo == 0)
                {
                    Fire(_aimInput, 1); // Regular shot
                    return;
                }

                // Charged shot
                FireChargedShots(_aimInput);
            }
        }
        else if (_hasStartedCharging) // Charging
        {
            if(Time.time - _timeForChargedShot >= Mathf.Lerp(_playerData.MaxTimeToChargeShot, _playerData.MinTimeToChargeShot, Mathf.Min(_chargedAmmo / (_playerData.ShotsTillFullCharge - 1.0f), 1.0f)))
            {
                if(_currentAmmo - _chargedAmmo <= 0)
                {
                    _hasStartedCharging = false; // Reached max charged Ammo
                    return;
                }

                ChargeAmmo();
                _timeForChargedShot = Time.time;
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
        if (_isReloading) // Currently _isReloading or can't fire
        {
            return;
        }

        // Start _isCharging a shot
        _timeForChargedShot = Time.time;

        _hasStartedCharging = true;
        _isInterrupted = false;
        _isCharging = true;

        _playerMovement.SetSpeedScale(_playerData.ChargeSlowDown);
        _soundManager.PlayGunChargeUp();
    }

    public void PlayerStopChargeInput(InputAction.CallbackContext context)
    {
        _hasStartedCharging = false;

        // Check if firing here as we can charge immediately but not fire immediately
        if (!CanFire() || !_isCharging || _chargedAmmo <= 0) 
        {
            _isCharging = false;
            if (!_isFiringChargedShot)
            {
                ReleaseChargedShots();
            }
            return; 
        }

        _isCharging = false;

        // Signals we want to fire
        _canTakeShot = true;
    }

    public void SetMouseAimInput(InputAction.CallbackContext context)
    {
        // Gather mouse position on screen to world position using the camera
        _isUsingMovementToAim = false;

        Vector2 inputValue = context.ReadValue<Vector2>();
        if(inputValue == new Vector2(transform.position.x, transform.position.y)) { return; }

        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(inputValue.x, inputValue.y, Camera.main.transform.position.z));
        Vector2 aimDirection = new Vector2(worldPoint.x - transform.position.x, worldPoint.y - transform.position.y);

        _aimInput = aimDirection.normalized;
        UpdateFacingDirection(_aimInput);
    }

    //public void SetControllerAimInput(InputAction.CallbackContext context)
    //{
    //    _isUsingMovementToAim = false;

    //    Vector2 inputValue = context.ReadValue<Vector2>();
    //    if (inputValue == Vector2.zero) { return; }

    //    _aimInput = inputValue;
    //    UpdateFacingDirection(_aimInput);
    //}

    public void SetAimToMovement(InputAction.CallbackContext context)
    {
        _isUsingMovementToAim = true;
    }

    public void UpdateAimDirectionToMovement(Vector2 direction)
    {
        if (_isUsingMovementToAim)
        {
            _aimInput = direction;
            UpdateFacingDirection(direction);
        }
    }

    public void PlayerFireInput(InputAction.CallbackContext context)
    {
        if(_isCharging || !CanFire()) { return; }
        _isInterrupted = false;
        _canTakeShot = true;
    }

    public void PlayerReloadAction(InputAction.CallbackContext context)
    {
        if(_isCharging || _isReloading || _currentAmmo == _playerData.MaxAmmo) { return; }

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
         * Not _isReloading
         */
        return (Time.time - _lastShotTime >= _playerData.FireRate - _debugData.FiringInputBuffer && !_isReloading);
    }

    private void Fire(Vector2 fireDirection, int ammoUsed, float fireMultiplier = 1.0f)
    {
        float rotz = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;

        // Create projectile and initialise it
        GameObject projectile = _objectPoolManager.GetFreeObject(_playerData.BaseProjectileType.name);
        projectile.transform.position = transform.position + new Vector3(fireDirection.normalized.x, fireDirection.normalized.y, 0)*2.0f;
        projectile.transform.rotation = Quaternion.Euler(0, 0, rotz);
        projectile.GetComponent<ProjectileBehaviour>().InitialiseComponent(fireDirection.normalized,
            _playerData.BaseProjectileSpeed,
            ref _objectPoolManager,
            _playerData.BaseProjectileType.name,
            fireMultiplier);

        // Sound
        _soundManager.PlayPGunFire();
        _isCocking = true;

        if (_hasCompanion)
        {
            projectile.GetComponent<ProjectileBehaviour>().AddCompanionTargetting(ref _companionManager);
        }

        _lastShotTime = Time.time;

        _currentAmmo -= ammoUsed;

        _bulletUIManager.UpdateLoadedBullets(_currentAmmo);

        if (_currentAmmo <= 0 && !_isReloading) // Trying to shoot with no ammo
        {
            StartCoroutine(ReloadAmmo());
            return;
        }
    }

    private IEnumerator ReloadAmmo()
    {
        _isReloading = true; // Let other functions know to not do anything

        _soundManager.PlayGunReload();

        _bulletUIManager.StartReloadAnim();

        float startTime = Time.time;

        while (Time.time - startTime <= _playerData.ReloadTime)
        {
            yield return null;
        }

        _currentAmmo = _playerData.MaxAmmo;

        _isReloading = false; // Let other functions run again

        _bulletUIManager.EndReloadAnim();

        _bulletUIManager.UpdateLoadedBullets(_currentAmmo);
    }

    public void InterruptFiring()
    {
        _isInterrupted = true;
        _hasStartedCharging = false;
        _isCharging = false;
        ReleaseChargedShots();
    }

    #endregion

    #region Charge Firing
    private void ChargeAmmo()
    {
        _chargedAmmo++;
        _bulletUIManager.UpdateChargedBulletsUI(_chargedAmmo);
    }

    // Fires one large shot for each charged bullet
    private void FireChargedShots(Vector2 direction)
    {
        float localDamageMultiplier = 1;

        for (int i = 0; i < _chargedAmmo; i++)
        {
            localDamageMultiplier *= _playerData.DamageMultiplier;
        }

        _soundManager.PlayGunChargeFire();

        float impactFrameTimescale = 1.0f + (((float)_chargedAmmo) * (_playerData.MaxChargeShotImpactSlowDown - 1.0f)) / ((float)_playerData.MaxAmmo);
        _timeManager.AddTimescaleForDuration(impactFrameTimescale, _playerData.ChargeShotImpactFrameDuration);

        Fire(direction, _chargedAmmo, localDamageMultiplier);

        ReleaseChargedShots();
    }

    private void ReleaseChargedShots()
    {
        _chargedAmmo = 0;

        _playerMovement.ResetSpeedScale();
        _bulletUIManager.UpdateLoadedBullets(_currentAmmo);
    }
    #endregion

    private void UpdateFacingDirection(Vector2 direction)
    {
        if(_lastAimInput == direction) { return; }
        _lastAimInput = direction;

        float AngleFromRight = Vector3.SignedAngle(Vector3.right, direction, new Vector3(0.0f, 0.0f, 1.0f));
        if (AngleFromRight > -45.0f && AngleFromRight < 45.0f) { _playerAnimationHandler.UpdateFacingDirection(PlayerAnimationHandler.FacingDirection.Right); }
        else if (AngleFromRight >= 45.0f && AngleFromRight <= 135.0f) { _playerAnimationHandler.UpdateFacingDirection(PlayerAnimationHandler.FacingDirection.Back); }
        else if (AngleFromRight > 135.0f || AngleFromRight < -135.0f) { _playerAnimationHandler.UpdateFacingDirection(PlayerAnimationHandler.FacingDirection.Left); }
        else if (AngleFromRight >= -135.0f || AngleFromRight <= -45.0f) { _playerAnimationHandler.UpdateFacingDirection(PlayerAnimationHandler.FacingDirection.Front); }

        _playerAnimationHandler.SetAimDirection(direction);
    }

    #region Gizmos
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));
        //Gizmos.DrawLine(transform.position, transform.position + new Vector3(worldPoint.x, worldPoint.y, 0.0f) * 10.0f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(_aimInput.x, _aimInput.y, 0.0f) * 10.0f);
    }
    #endregion
}
