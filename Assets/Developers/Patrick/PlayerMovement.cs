using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private float horizontalInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x + horizontalInput * Time.deltaTime, transform.position.y, transform.position.z);
    }

    public void SetHorizontalMovementInput(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<float>();
    }
}
