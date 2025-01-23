using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public PlayerInputs_ActionMap actionMap;
    private InputAction horizontalMovement;

    public PlayerMovement playerMovement;

    public bool isInitialised = false;

    public void setInitialised(bool state) { isInitialised = state; }

    // Start is called before the first frame update
    void Start()
    {
        actionMap = new PlayerInputs_ActionMap();
        horizontalMovement = actionMap.Movement.HorizontalMovement;
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
        horizontalMovement.Enable();
        horizontalMovement.performed += playerMovement.SetHorizontalMovementInput;
        horizontalMovement.canceled += playerMovement.SetHorizontalMovementInput;

    }

    public void DisableInput()
    {
        horizontalMovement.Disable();
        horizontalMovement.performed -= playerMovement.SetHorizontalMovementInput;
        horizontalMovement.canceled -= playerMovement.SetHorizontalMovementInput;
    }
}
