using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    private PlayerManager _playerManager;
    private PlayerShooting _playerShooting;
    private PlayerAnimationHandler _playerAnimationHandler;
    private Rigidbody2D rb;
    private GameObject UICanvas;
    private SoundManager _soundManager;
    private HealthBarScript healthBarScript;

    private Vector2 movementInput;

    private GameObject[] dashChargesUIObjects;
    private GameObject[] dashRechargesUIObjects;

    private GameObject _evolveDashCollider;

    public bool dash = false;
    private bool _isDashStarted = false;
    private float dashTimer = 0.0f;
    private int dashChargesNumber = 0;
    private int maxDashCharges = 0;
    private float dashChargeTimer = 0.0f;
    private Vector2 movementDirection = Vector2.up;
    private Vector2 dashStart = Vector2.zero;
    private Vector2 dashDirection = Vector2.zero;
    private float lastDashTime = -10.0f;
    private float lastDashInputTime = -10.0f;
    private bool _isMoving = false;
    private bool _isMovingLastState = false;
    private float _speedScale = 1.0f;

    private Vector2 knockbackForce = Vector2.zero;

    private bool evolved = false;

    private PlayerData _playerData;
    private PlayerDebugData _debugData;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();        
    }

    public void InitialiseComponent(ref PlayerManager playerManager, ref PlayerShooting playerShooting, ref PlayerAnimationHandler playerAnimationHandler, ref PlayerData playerData, ref PlayerDebugData debugData, ref GameObject dUICanvas, ref SoundManager soundManager, ref HealthBarScript dHealthbarScript, ref GameObject evolveDashCollider)
    {
        _playerManager = playerManager;
        _playerShooting = playerShooting;

        _playerAnimationHandler = playerAnimationHandler;

        UICanvas = dUICanvas;

        _playerData = playerData;
        _debugData = debugData;

        _soundManager = soundManager;

        healthBarScript = dHealthbarScript;

        _evolveDashCollider = evolveDashCollider;

        dashChargesNumber = 1;
        maxDashCharges = 1;

        healthBarScript.setDashUI(dashChargesNumber);

        //EvolveDash(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(dashChargesNumber < maxDashCharges)
        {
            dashChargeTimer += Time.deltaTime;
        }
        if(dashChargeTimer >= _playerData.dashRechargeTime)
        {
            dashChargeTimer = 0.0f;

            if (dashChargesNumber < maxDashCharges)
            {
                dashChargesNumber = maxDashCharges;
                healthBarScript.setDashUI(dashChargesNumber);
            }
        }

        if (_isMoving != _isMovingLastState)
        {
            _soundManager.SetWalking(_isMoving);
            _isMovingLastState = _isMoving;
        }
    }

    private void FixedUpdate()
    {
        if (movementInput != Vector2.zero)
        {
            // Do we need this?
            movementDirection = movementInput.normalized;
            _playerShooting.UpdateAimDirectionToMovement(movementDirection);
        }

        if (knockbackForce != Vector2.zero)
        {
            dash = false;

            rb.AddForce(knockbackForce, ForceMode2D.Impulse);
            knockbackForce = Vector2.zero;
            _isMoving = false;

            _playerAnimationHandler.UpdateMovementAnimation(false);
            return;
        }

        if (dash)
        {
            if (Time.time - lastDashTime >= _playerData.dashCooldown && dashChargesNumber > 0) // Off cooldown
            {
                if (!_isDashStarted)
                {
                    _isDashStarted = true;
                    _soundManager.SetDashing(true);
                    if (evolved)
                    {
                        _evolveDashCollider.SetActive(true);
                    }

                    _playerAnimationHandler.StartDashAnimation();
                }

                dashChargeTimer = 0.0f;
                dashTimer += Time.fixedDeltaTime;

                _playerManager.SetDashInvulnerability(true);
                // Update evolve dash collision layers if active

                rb.MovePosition(Vector2.Lerp(dashStart, dashStart + dashDirection * (evolved ? _playerData.dashDistance + _playerData.evolvedDashExtraDistance : _playerData.dashDistance), Mathf.Min(dashTimer / _playerData.dashTime, 1.0f)));

                if (dashTimer >= _playerData.dashTime)
                {
                    _playerManager.SetDashInvulnerability(false);

                    dash = false;

                    lastDashTime = Time.time;

                    dashChargesNumber--;
                    healthBarScript.setDashUI(dashChargesNumber);
                    _soundManager.SetDashing(false);
                    if (evolved)
                    {
                        _evolveDashCollider.SetActive(false);
                    }

                    _playerAnimationHandler.EndDashAnimation();
                }

                _isMoving = false;

                return;
            }
            else if (Time.time - lastDashInputTime > _debugData.dashInputBuffer) // Buffer has been exceeded
            {
                _soundManager.PlayDashEmpty();
                dash = false;
            }

        }

        Movement();

        _playerAnimationHandler.UpdateMovementAnimation(_isMoving);
    }

    private void Movement()
    {
        _isMoving = (movementInput != Vector2.zero);

        // Get the speed the player wants to move at
        Vector2 targetSpeed = movementInput * _playerData.speed * _speedScale;

        // Calculate acceleration type
        float accelerationRate = (targetSpeed.sqrMagnitude > 0.0001f) ? _playerData.accelerationRate : _playerData.decelerationRate;

        Vector2 speedDiff = targetSpeed - rb.velocity;

        Vector2 movement =  speedDiff * accelerationRate;

        rb.AddForce(movement, ForceMode2D.Force);
    }

    public Vector2 GetMovementDirection()
    {
        return movementDirection;
    }

    public void SetMovementInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void PlayerDashInput(InputAction.CallbackContext context)
    {

        lastDashInputTime = Time.time;
        if (!dash)
        {
            dash = true;
            dashTimer = 0.0f;
            dashStart = new Vector2(transform.position.x, transform.position.y);
            dashDirection = movementDirection;

            _isDashStarted = false;
        }
    }

    public void KnockbackPlayer(Vector2 direction, float scale)
    {
        knockbackForce = direction * scale;
    }

    IEnumerator DashColor()
    {
        SpriteRenderer spriteRenderer = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.color = Color.cyan;
        yield return new WaitForSeconds(_playerData.dashTime);
        if (spriteRenderer.color == Color.cyan)
        {
            spriteRenderer.color = Color.white;
        }
    }

    public void EvolveDash(bool toggle)
    {
        if (toggle)
        {
            maxDashCharges = _playerData.numberOfDashCharges;
            dashChargesNumber = maxDashCharges;
            evolved = true;
        }
        else
        {
            maxDashCharges = 1;
            dashChargesNumber = maxDashCharges;
            evolved = false;
        }
        
    }

    public void SetSpeedScale(float scale)
    {
        _speedScale = scale;
        _playerAnimationHandler.UpdateMovementScale(scale);
    }

    public void ResetSpeedScale()
    {
        _speedScale = 1.0f;
        _playerAnimationHandler.UpdateMovementScale(1.0f);
    }

    public void RechargeDashes(int n = 3)
    {
        if (maxDashCharges != _playerData.numberOfDashCharges)
        {
            if(n > 1)
            {
                n = 1;
            }
        }

        dashChargesNumber += n;

        if(dashChargesNumber > _playerData.numberOfDashCharges)
        {
            dashChargesNumber = _playerData.numberOfDashCharges;
            dashChargeTimer = 0.0f;
        }

        healthBarScript.setDashUI(dashChargesNumber);
    }

    public Vector2 getMovementInput()
    {
        return movementInput;
    }
}
