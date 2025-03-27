using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractComponent : MonoBehaviour
{
    private bool interact = false;
    private float interactTimer = 1.0f;

    private void Update()
    {
        if (interactTimer > 0)
        {
            interactTimer -= Time.deltaTime;
        }
        if (interactTimer <= 0 && interact == true)
        {
            interact = false;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        interact = true;
        interactTimer = 1.0f;
    }

    public bool GetInteract()
    {
        return interact;
    }
}
