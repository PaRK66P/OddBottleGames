using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public PlayerInputs_ActionMap actionMap;
    private InputAction movementAction;
    private InputAction dashAction;
    private InputAction fireAction;
    private InputAction reloadAction;
    private InputAction chargeAction;

    private InputAction aimAction;

    public PlayerMovement playerMovement;
    public PlayerShooting playerShooting;

    public bool isInitialised = false;

    public void setInitialised(bool state) { isInitialised = state; }

    // Start is called before the first frame update
    public void InitialiseComponent(ref PlayerMovement dPlayerMovement, ref PlayerShooting dPlayerShooting)
    {
        actionMap = new PlayerInputs_ActionMap();
        movementAction = actionMap.Player.Movement;
        dashAction = actionMap.Player.Dash;
        fireAction = actionMap.Player.Shoot;
        reloadAction = actionMap.Player.Reload;
        chargeAction = actionMap.Player.Charge;

        aimAction = actionMap.Player.Aim;

        playerMovement = dPlayerMovement;
        playerShooting = dPlayerShooting;

        isInitialised = true;
    }

    void OnEnable()
    {
        if (isInitialised) { actionMap.Enable(); EnableInput(); }
    }

    private void OnDisable()
    {
        DisableInput();
        actionMap.Disable();
    }

    public void EnableInput()
    {
        movementAction.Enable();
        dashAction.Enable();
        fireAction.Enable();
        reloadAction.Enable();
        chargeAction.Enable();

        aimAction.Enable();

        movementAction.performed += playerMovement.SetMovementInput;
        movementAction.canceled += playerMovement.SetMovementInput;
        dashAction.performed += playerMovement.PlayerDashInput;
        chargeAction.performed += playerShooting.PlayerChargeInput;
        chargeAction.canceled += playerShooting.PlayerStopChargeInput;
        fireAction.performed += playerShooting.PlayerFireInput;
        reloadAction.performed += playerShooting.PlayerReloadAction;
    }

    public void DisableInput()
    {
        movementAction.Disable();
        dashAction.Disable();
        fireAction.Disable();
        reloadAction.Disable();
        chargeAction.Disable();

        aimAction.Disable();

        movementAction.performed -= playerMovement.SetMovementInput;
        movementAction.canceled -= playerMovement.SetMovementInput;
        dashAction.performed -= playerMovement.PlayerDashInput;
        chargeAction.performed -= playerShooting.PlayerChargeInput;
        chargeAction.canceled -= playerShooting.PlayerStopChargeInput;
        fireAction.performed -= playerShooting.PlayerFireInput;
        reloadAction.performed -= playerShooting.PlayerReloadAction;
    }
}
