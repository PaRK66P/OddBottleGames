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
    private InputAction interactAction;

    public PlayerMovement playerMovement;
    public PlayerShooting playerShooting;
    public InteractComponent interactComponent;

    public bool isInitialised = false;

    public void setInitialised(bool state) { isInitialised = state; }

    // Start is called before the first frame update
    public void InitialiseComponent(ref PlayerMovement dPlayerMovement, ref PlayerShooting dPlayerShooting, ref InteractComponent dInteractComponent)
    {
        actionMap = new PlayerInputs_ActionMap();
        movementAction = actionMap.Player.Movement;
        dashAction = actionMap.Player.Dash;
        fireAction = actionMap.Player.Shoot;
        reloadAction = actionMap.Player.Reload;
        interactAction = actionMap.Player.Interact;

        playerMovement = dPlayerMovement;
        playerShooting = dPlayerShooting;
        interactComponent = dInteractComponent;

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
        interactAction.Enable();

        movementAction.performed += playerMovement.SetMovementInput;
        movementAction.canceled += playerMovement.SetMovementInput;
        dashAction.performed += playerMovement.PlayerDashInput;
        fireAction.performed += playerShooting.PlayerFireInput;
        fireAction.canceled += playerShooting.PlayerStopFireInput;
        reloadAction.performed += playerShooting.PlayerReloadAction;
        interactAction.performed += interactComponent.OnInteract;
    }

    public void DisableInput()
    {
        movementAction.Disable();
        dashAction.Disable();
        fireAction.Disable();
        reloadAction.Disable();
        interactAction.Disable();

        movementAction.performed -= playerMovement.SetMovementInput;
        movementAction.canceled -= playerMovement.SetMovementInput;
        dashAction.performed -= playerMovement.PlayerDashInput;
        fireAction.performed -= playerShooting.PlayerFireInput;
        fireAction.canceled -= playerShooting.PlayerStopFireInput;
        reloadAction.performed -= playerShooting.PlayerReloadAction;
        interactAction.performed -= interactComponent.OnInteract;
    }
}
