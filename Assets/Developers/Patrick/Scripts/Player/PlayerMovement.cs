using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    // Objects
    private PlayerData _playerData;
    private PlayerDebugData _debugData;
    private SoundManager _soundManager;
    private HealthBarScript _healthBarScript;

    // Components
    private PlayerManager _playerManager;
    private PlayerShooting _playerShooting;
    private PlayerAnimationHandler _playerAnimationHandler;
    private Rigidbody2D _rigidbody;
    private GameObject _evolveDashCollider;

    // Values
    private Vector2 _movementInput;

    public bool _canDash = false;
    private bool _isDashStarted = false;
    private float _dashTimer = 0.0f;
    private int _dashChargesNumber = 0;
    private int _maxDashCharges = 0;
    private float _dashChargeTimer = 0.0f;
    private Vector2 _movementDirection = Vector2.up;
    private Vector2 _dashStart = Vector2.zero;
    private Vector2 _dashDirection = Vector2.zero;
    private float _lastDashTime = -10.0f;
    private float lastDashInputTime = -10.0f;
    private bool _isMoving = false;
    private bool _isMovingLastState = false;
    private float _speedScale = 1.0f;

    private Vector2 _knockbackForce = Vector2.zero;

    private bool _hasEvolved = false;


    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();        
    }

    public void InitialiseComponent(ref PlayerManager playerManager, ref PlayerShooting playerShooting, ref PlayerAnimationHandler playerAnimationHandler, ref PlayerData playerData, ref PlayerDebugData debugData, ref SoundManager soundManager, ref HealthBarScript healthbarScript, ref GameObject evolveDashCollider)
    {
        _playerManager = playerManager;
        _playerShooting = playerShooting;

        _playerAnimationHandler = playerAnimationHandler;

        _playerData = playerData;
        _debugData = debugData;

        _soundManager = soundManager;

        _healthBarScript = healthbarScript;

        _evolveDashCollider = evolveDashCollider;

        _dashChargesNumber = 1;
        _maxDashCharges = 1;

        _healthBarScript.setDashUI(_dashChargesNumber);
    }

    // Update is called once per frame
    void Update()
    {
        // Recharge dashes
        if(_dashChargesNumber < _maxDashCharges)
        {
            _dashChargeTimer += Time.deltaTime;
        }
        if(_dashChargeTimer >= _playerData.DashRechargeTime)
        {
            _dashChargeTimer = 0.0f;

            if (_dashChargesNumber < _maxDashCharges)
            {
                _dashChargesNumber = _maxDashCharges;
                _healthBarScript.setDashUI(_dashChargesNumber);
            }
        }

        // Sound - Footsteps
        if (_isMoving != _isMovingLastState)
        {
            _soundManager.SetWalking(_isMoving);
            _isMovingLastState = _isMoving;
        }
    }

    private void FixedUpdate()
    {
        // Update movement direction
        if (_movementInput != Vector2.zero)
        {
            // Do we need this?
            // - YES
            _movementDirection = _movementInput.normalized;
            _playerShooting.UpdateAimDirectionToMovement(_movementDirection);
        }

        // Apply knockback
        if (_knockbackForce != Vector2.zero)
        {
            _canDash = false;

            _rigidbody.AddForce(_knockbackForce, ForceMode2D.Impulse);
            _knockbackForce = Vector2.zero;
            _isMoving = false;

            _playerAnimationHandler.UpdateMovementAnimation(false);
            return;
        }

        // Dashing
        if (_canDash) // Can dashing means wanting to dash
        {
            if (Time.time - _lastDashTime >= _playerData.DashCooldown && _dashChargesNumber > 0) // Off cooldown
            {
                if (!_isDashStarted)
                {
                    _isDashStarted = true;
                    _soundManager.SetDashing(true);
                    if (_hasEvolved)
                    {
                        _evolveDashCollider.SetActive(true);
                    }

                    _playerAnimationHandler.StartDashAnimation();
                }

                // Dash is handled over time rather than a force

                _dashChargeTimer = 0.0f; // Cooldown
                _dashTimer += Time.fixedDeltaTime;

                _playerManager.SetDashInvulnerability(true);

                _rigidbody.MovePosition(Vector2.Lerp(_dashStart, _dashStart + _dashDirection * (_hasEvolved ? _playerData.DashDistance + _playerData.EvolvedDashExtraDistance : _playerData.DashDistance), Mathf.Min(_dashTimer / _playerData.DashTime, 1.0f)));

                if (_dashTimer >= _playerData.DashTime) // Dash ended
                {
                    _playerManager.SetDashInvulnerability(false);

                    _canDash = false;

                    _lastDashTime = Time.time;

                    _dashChargesNumber--;
                    _healthBarScript.setDashUI(_dashChargesNumber);
                    _soundManager.SetDashing(false);
                    if (_hasEvolved)
                    {
                        _evolveDashCollider.SetActive(false);
                    }

                    _playerAnimationHandler.EndDashAnimation();
                }

                _isMoving = false;

                return;
            }
            else if (Time.time - lastDashInputTime > _debugData.DashInputBuffer) // Buffer has been exceeded
            {
                _soundManager.PlayDashEmpty();
                _canDash = false;
            }
        }

        // Movement
        Movement();
        _playerAnimationHandler.UpdateMovementAnimation(_isMoving);
    }

    // How movement is being handled
    /*
     * The aim is for movement to be applied based on what the player *wants* to achieve
     */
    private void Movement()
    {
        _isMoving = (_movementInput != Vector2.zero);

        // Get the speed the player wants to move at
        Vector2 targetSpeed = _movementInput * _playerData.Speed * _speedScale;

        // Calculate acceleration type
        float accelerationRate = (targetSpeed.sqrMagnitude > 0.0001f) ? _playerData.AccelerationRate : _playerData.DecelerationRate;

        Vector2 speedDiff = targetSpeed - _rigidbody.velocity;

        Vector2 movement =  speedDiff * accelerationRate;

        _rigidbody.AddForce(movement, ForceMode2D.Force);
    }

    public Vector2 GetMovementDirection()
    {
        return _movementDirection;
    }

    public void SetMovementInput(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
    }

    public void PlayerDashInput(InputAction.CallbackContext context)
    {
        lastDashInputTime = Time.time;
        if (!_canDash) // Not already trying to dash
        {
            _canDash = true;
            _dashTimer = 0.0f;
            _dashStart = new Vector2(transform.position.x, transform.position.y);
            _dashDirection = _movementDirection;

            _isDashStarted = false;
        }
    }

    public void KnockbackPlayer(Vector2 direction, float scale)
    {
        _knockbackForce = direction * scale;
    }

    // Evolve the dash ability (toggle can disable the dash evolve)
    /*
     * Grants the player three dash charges
     * Dash also deals damage (handled in manager)
     */
    public void EvolveDash(bool toggle)
    {
        if (toggle)
        {
            _maxDashCharges = _playerData.NumberOfDashCharges;
            _dashChargesNumber = _maxDashCharges;
            _hasEvolved = true;
        }
        else
        {
            _maxDashCharges = 1;
            _dashChargesNumber = _maxDashCharges;
            _hasEvolved = false;
        }
    }

    // Scale of movement animation
    public void SetSpeedScale(float scale)
    {
        _speedScale = scale;
        _playerAnimationHandler.UpdateMovementScale(scale / _playerData.RunAnimationSpeedScale);
    }

    // Resets to base amount
    public void ResetSpeedScale()
    {
        _speedScale = 1.0f;
        _playerAnimationHandler.UpdateMovementScale(_playerData.RunAnimationSpeedScale);
    }

    // Regain all dash charges
    public void RechargeDashes(int n = 3)
    {
        if (_maxDashCharges != _playerData.NumberOfDashCharges)
        {
            if(n > 1)
            {
                n = 1;
            }
        }

        _dashChargesNumber += n;

        if(_dashChargesNumber > _playerData.NumberOfDashCharges)
        {
            _dashChargesNumber = _playerData.NumberOfDashCharges;
            _dashChargeTimer = 0.0f;
        }

        _healthBarScript.setDashUI(_dashChargesNumber);
    }

    public Vector2 getMovementInput()
    {
        return _movementInput;
    }
}
