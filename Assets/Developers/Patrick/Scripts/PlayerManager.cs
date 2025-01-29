using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement playerMovement;
    [SerializeField]
    private PlayerInputManager playerInputManager;

    [SerializeField]
    private GameObject weaponDrop;

    private bool hasWeapon = true;

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

    public void TakeDamage()
    {
        if (hasWeapon && !playerMovement.dash)
        {
            hasWeapon = false;
            playerMovement.DisableFire();

            float randomAngle = Random.Range(0, 360);
            Vector3 weaponPos = new Vector3(5 * Mathf.Cos(randomAngle), 5 * Mathf.Sin(randomAngle)) + transform.position;
            Instantiate(weaponDrop, weaponPos, Quaternion.identity);
        }
        
    }

    public void RegainWeapon()
    {
        hasWeapon = true;
        playerMovement.EnableFire();
    }
}
