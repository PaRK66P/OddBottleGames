using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public PlayerInputs_ActionMap actionMap;
    private InputAction movementAction;

    public PlayerMovement playerMovement;

    public bool isInitialised = false;

    public void setInitialised(bool state) { isInitialised = state; }

    // Start is called before the first frame update
    void Start()
    {
        actionMap = new PlayerInputs_ActionMap();
        movementAction = actionMap.Movement.Movement;
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
        movementAction.performed += playerMovement.SetMovementInput;
        movementAction.canceled += playerMovement.SetMovementInput;


    }

    public void DisableInput()
    {
        movementAction.Disable();
        movementAction.performed -= playerMovement.SetMovementInput;
        movementAction.canceled -= playerMovement.SetMovementInput;
    }
}
