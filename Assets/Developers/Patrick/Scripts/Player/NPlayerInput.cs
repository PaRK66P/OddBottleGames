using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NPlayerInput : MonoBehaviour
{
    // Start is called before the first frame update

    private PlayerInput playerIn;
    public PlayerMovement playerMovement;
    public PlayerShooting playerShooting;

    private NewPlayerInputMap playerInputActions;

    private bool isInitialised = false;
    private bool isUsingController = true;

    void OnEnable()
    {
        // Might not be necessary
        if (isInitialised) 
        { 
            EnableInput();
            playerIn.onControlsChanged += OnChangeControls;
        }
    }

    private void OnDisable()
    {
        // Might not be necessary
        DisableInput();
        playerIn.onControlsChanged -= OnChangeControls;
    }

    public void InitialiseComponent(ref PlayerMovement dPlayerMovement, ref PlayerShooting dPlayerShooting)
    {
        playerIn = GetComponent<PlayerInput>();
        playerIn.onControlsChanged += OnChangeControls;

        playerMovement = dPlayerMovement;
        playerShooting = dPlayerShooting;

        playerInputActions = new NewPlayerInputMap();

        isInitialised = true;

        EnableInput();
    }

    private void Update()
    {
        
    }

    public void OnChangeControls(PlayerInput input)
    {
        if(input.currentControlScheme == "Controller")
        {
            isUsingController = true;
            EnableControllerAim();
            DisableMouseAim();
        }
        else if(input.currentControlScheme == "Keyboard")
        {
            isUsingController = false;
            EnableMouseAim();
            DisableControllerAim();
        }
        else
        {
            Debug.LogWarning("Unknown device");
        }
    }

    public void EnableInput()
    {
        playerInputActions.Player.Movement.Enable();
        playerInputActions.Player.Dash.Enable();
        playerInputActions.Player.Shoot.Enable();
        playerInputActions.Player.Reload.Enable();
        playerInputActions.Player.Charge.Enable();

        playerInputActions.Player.Movement.performed += playerMovement.SetMovementInput;
        playerInputActions.Player.Movement.canceled += playerMovement.SetMovementInput;
        playerInputActions.Player.Dash.performed += playerMovement.PlayerDashInput;
        playerInputActions.Player.Charge.performed += playerShooting.PlayerChargeInput;
        playerInputActions.Player.Charge.canceled += playerShooting.PlayerStopChargeInput;
        playerInputActions.Player.Shoot.performed += playerShooting.PlayerFireInput;
        playerInputActions.Player.Reload.performed += playerShooting.PlayerReloadAction;

        if (isUsingController)
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
        playerInputActions.Player.Movement.Disable();
        playerInputActions.Player.Dash.Disable();
        playerInputActions.Player.Shoot.Disable();
        playerInputActions.Player.Reload.Disable();
        playerInputActions.Player.Charge.Disable();

        playerInputActions.Player.AimDirection.Disable();
        playerInputActions.Player.AimPosition.Disable();

        playerInputActions.Player.Movement.performed -= playerMovement.SetMovementInput;
        playerInputActions.Player.Movement.canceled -= playerMovement.SetMovementInput;
        playerInputActions.Player.Dash.performed -= playerMovement.PlayerDashInput;
        playerInputActions.Player.Charge.performed -= playerShooting.PlayerChargeInput;
        playerInputActions.Player.Charge.canceled -= playerShooting.PlayerStopChargeInput;
        playerInputActions.Player.Shoot.performed -= playerShooting.PlayerFireInput;
        playerInputActions.Player.Reload.performed -= playerShooting.PlayerReloadAction;

        if (isUsingController)
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
        playerInputActions.Player.AimDirection.Enable();

        playerInputActions.Player.AimDirection.performed += playerShooting.SetControllerAimInput;
        //playerInputActions.Player.AimDirection.canceled += playerShooting.SetControllerAimInput;
    }

    private void DisableControllerAim()
    {
        playerInputActions.Player.AimDirection.Disable();

        playerInputActions.Player.AimDirection.performed -= playerShooting.SetControllerAimInput;
        //playerInputActions.Player.AimDirection.canceled -= playerShooting.SetControllerAimInput;
    }
    private void EnableMouseAim()
    {
        playerInputActions.Player.AimPosition.Enable();

        playerInputActions.Player.AimPosition.performed += playerShooting.SetMouseAimInput;
        //playerInputActions.Player.AimPosition.canceled += playerShooting.SetMouseAimInput;
    }

    private void DisableMouseAim()
    {
        playerInputActions.Player.AimPosition.Disable();

        playerInputActions.Player.AimPosition.performed -= playerShooting.SetMouseAimInput;
        //playerInputActions.Player.AimPosition.canceled -= playerShooting.SetMouseAimInput;
    }

    public void setInitialised(bool state) { isInitialised = state; }

    public bool isInputKeyboard()
    {
        if(playerIn.currentControlScheme == "Keyboard")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
