using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    // Objects
    private NewPlayerInputMap _playerInputActions;

    // Components
    private PlayerInput _playerInput;
    private PlayerMovement _playerMovement;
    private PlayerShooting _playerShooting;
    private PlayerAimReticle _playerAimReticle;

    // Values
    private bool _isInitialised = false;
    private bool _isUsingController = true;

    void OnEnable()
    {
        // Might not be necessary
        if (_isInitialised)
        {
            EnableInput();
            _playerInput.onControlsChanged += OnChangeControls;
        }
    }

    private void OnDisable()
    {
        // Might not be necessary
        DisableInput();
        _playerInput.onControlsChanged -= OnChangeControls;
    }

    public void InitialiseComponent(ref PlayerMovement playerMovement, ref PlayerShooting playerShooting, ref PlayerAimReticle playerAimReticle)
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.onControlsChanged += OnChangeControls;

        _playerMovement = playerMovement;
        _playerShooting = playerShooting;
        _playerAimReticle = playerAimReticle;

        _playerInputActions = new NewPlayerInputMap();

        _isInitialised = true;

        // Setup control scheme
        if (_playerInput.currentControlScheme == "Keyboard")
        {
            _isUsingController = false;
            playerAimReticle.SwitchToMouse();
        }
        else
        {
            _isUsingController = true;
            playerAimReticle.SwitchToController();
        }

        EnableInput();
    }

    public void OnChangeControls(PlayerInput input)
    {
        if (input.currentControlScheme == "Controller")
        {
            _isUsingController = true;
            EnableControllerAim();
            DisableMouseAim();
            _playerAimReticle.SwitchToController();
        }
        else if (input.currentControlScheme == "Keyboard")
        {
            _isUsingController = false;
            EnableMouseAim();
            DisableControllerAim();
            _playerAimReticle.SwitchToMouse();
        }
        else
        {
            Debug.LogWarning("Unknown device");
        }
    }

    public void EnableInput()
    {
        // Enable Action
        _playerInputActions.Player.Movement.Enable();
        _playerInputActions.Player.Dash.Enable();
        _playerInputActions.Player.Shoot.Enable();
        _playerInputActions.Player.Reload.Enable();
        _playerInputActions.Player.Charge.Enable();

        // Bind input functions
        _playerInputActions.Player.Movement.performed += _playerMovement.SetMovementInput;
        _playerInputActions.Player.Movement.canceled += _playerMovement.SetMovementInput;
        _playerInputActions.Player.Dash.performed += _playerMovement.PlayerDashInput;
        _playerInputActions.Player.Charge.performed += _playerShooting.PlayerChargeInput;
        _playerInputActions.Player.Charge.canceled += _playerShooting.PlayerStopChargeInput;
        _playerInputActions.Player.Shoot.performed += _playerShooting.PlayerFireInput;
        _playerInputActions.Player.Reload.performed += _playerShooting.PlayerReloadAction;

        if (_isUsingController)
        {
            EnableControllerAim();
        }
        else
        {
            EnableMouseAim();
        }
    }

    public void DisableInput()
    {
        // Disable Actions
        _playerInputActions.Player.Movement.Disable();
        _playerInputActions.Player.Dash.Disable();
        _playerInputActions.Player.Shoot.Disable();
        _playerInputActions.Player.Reload.Disable();
        _playerInputActions.Player.Charge.Disable();

        _playerInputActions.Player.AimDirection.Disable();
        _playerInputActions.Player.AimPosition.Disable();

        // Remove functions
        _playerInputActions.Player.Movement.performed -= _playerMovement.SetMovementInput;
        _playerInputActions.Player.Movement.canceled -= _playerMovement.SetMovementInput;
        _playerInputActions.Player.Dash.performed -= _playerMovement.PlayerDashInput;
        _playerInputActions.Player.Charge.performed -= _playerShooting.PlayerChargeInput;
        _playerInputActions.Player.Charge.canceled -= _playerShooting.PlayerStopChargeInput;
        _playerInputActions.Player.Shoot.performed -= _playerShooting.PlayerFireInput;
        _playerInputActions.Player.Reload.performed -= _playerShooting.PlayerReloadAction;

        if (_isUsingController)
        {
            DisableControllerAim();
        }
        else
        {
            DisableMouseAim();
        }
    }

    private void EnableControllerAim()
    {
        _playerInputActions.Player.AimDirection.Enable();

        _playerInputActions.Player.AimDirection.performed += _playerShooting.SetControllerAimInput;
        _playerInputActions.Player.AimDirection.canceled += _playerShooting.SetAimToMovement;
    }

    private void DisableControllerAim()
    {
        _playerInputActions.Player.AimDirection.Disable();

        _playerInputActions.Player.AimDirection.performed -= _playerShooting.SetControllerAimInput;
        _playerInputActions.Player.AimDirection.canceled -= _playerShooting.SetAimToMovement;
    }
    private void EnableMouseAim()
    {
        _playerInputActions.Player.AimPosition.Enable();

        _playerInputActions.Player.AimPosition.performed += _playerShooting.SetMouseAimInput;
    }

    private void DisableMouseAim()
    {
        _playerInputActions.Player.AimPosition.Disable();

        _playerInputActions.Player.AimPosition.performed -= _playerShooting.SetMouseAimInput;
    }

    public void SetInitialised(bool state) { _isInitialised = state; }

    public bool IsInputKeyboard()
    {
        if (_playerInput.currentControlScheme == "Keyboard")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
