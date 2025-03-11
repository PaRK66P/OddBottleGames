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

    public void InitialiseComponent(ref PlayerMovement dPlayerMovement, ref PlayerShooting dPlayerShooting)
    {
        playerIn = GetComponent<PlayerInput>();

        playerMovement = dPlayerMovement;
        playerShooting = dPlayerShooting;

        playerInputActions = new NewPlayerInputMap();

        isInitialised = true;
    }

    public void EnableInput()
    {
        playerInputActions.Player.Movement.Enable();
        playerInputActions.Player.Dash.Enable();
        playerInputActions.Player.Shoot.Enable();
        playerInputActions.Player.Reload.Enable();
        playerInputActions.Player.Charge.Enable();

        playerInputActions.Player.Aim.Enable();

        playerInputActions.Player.Movement.performed += playerMovement.SetMovementInput;
        playerInputActions.Player.Movement.canceled += playerMovement.SetMovementInput;
        playerInputActions.Player.Dash.performed += playerMovement.PlayerDashInput;
        playerInputActions.Player.Charge.performed += playerShooting.PlayerChargeInput;
        playerInputActions.Player.Charge.canceled += playerShooting.PlayerStopChargeInput;
        playerInputActions.Player.Shoot.performed += playerShooting.PlayerFireInput;
        playerInputActions.Player.Reload.performed += playerShooting.PlayerReloadAction;

        playerInputActions.Player.Aim.performed += playerShooting.SetAimInput;
        playerInputActions.Player.Aim.canceled += playerShooting.SetAimInput;
    }

    public void DisableInput()
    {
        playerInputActions.Player.Movement.Disable();
        playerInputActions.Player.Dash.Disable();
        playerInputActions.Player.Shoot.Disable();
        playerInputActions.Player.Reload.Disable();
        playerInputActions.Player.Charge.Disable();

        playerInputActions.Player.Aim.Disable();

        playerInputActions.Player.Movement.performed -= playerMovement.SetMovementInput;
        playerInputActions.Player.Movement.canceled -= playerMovement.SetMovementInput;
        playerInputActions.Player.Dash.performed -= playerMovement.PlayerDashInput;
        playerInputActions.Player.Charge.performed -= playerShooting.PlayerChargeInput;
        playerInputActions.Player.Charge.canceled -= playerShooting.PlayerStopChargeInput;
        playerInputActions.Player.Shoot.performed -= playerShooting.PlayerFireInput;
        playerInputActions.Player.Reload.performed -= playerShooting.PlayerReloadAction;

        playerInputActions.Player.Aim.performed -= playerShooting.SetAimInput;
        playerInputActions.Player.Aim.canceled -= playerShooting.SetAimInput;
    }

    public void setInitialised(bool state) { isInitialised = state; }

    // Update is called once per frame
    void Update()
    {
        
    }
}
