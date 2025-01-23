using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement playerMovement;
    [SerializeField]
    private PlayerInputManager playerInputManager;

    // Start is called before the first frame update
    void Start()
    {
        playerInputManager.playerMovement = playerMovement;
        playerInputManager.isInitialised = true;
        playerInputManager.EnableInput();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
