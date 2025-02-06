using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private PlayerData playerData;
    [SerializeField]
    private PlayerDebugData debugData;

    private PlayerInputManager playerInputManager;
    private PlayerMovement playerMovement;
    private PlayerShooting playerShooting;

    private GameObject weaponDrop;

    private bool hasWeapon = true;

    private event EventHandler OnDamageTaken;

    // Start is called before the first frame update
    void Start()
    {
        playerInputManager = gameObject.AddComponent<PlayerInputManager>();
        playerMovement = gameObject.AddComponent<PlayerMovement>();
        playerShooting = gameObject.AddComponent<PlayerShooting>();

        playerInputManager.InitialiseComponent(ref playerMovement, ref playerShooting);
        playerMovement.InitialiseComponent(playerData.speed, playerData.dashTime, playerData.dashDistance, playerData.damageLayers);
        playerShooting.InitialiseComponent(playerData.baseProjectileType, playerData.baseProjectileSpeed, debugData.canDropWeapon);

        playerInputManager.EnableInput();

        if (debugData.canDropWeapon)
        {
            OnDamageTaken += DropWeapon;
        }
    }

    private void OnDisable()
    {
        // Improper event subscription, need to update later
        if (debugData.canDropWeapon)
        {
            OnDamageTaken -= DropWeapon;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage()
    {
        OnDamageTaken?.Invoke(this, EventArgs.Empty);
    }

    private void DropWeapon(object sender, EventArgs e)
    {
        if (hasWeapon && !playerMovement.dash)
        {
            hasWeapon = false;
            playerShooting.DisableFire();

            float randomAngle = UnityEngine.Random.Range(0, 360);
            Vector3 weaponPos = new Vector3(5 * Mathf.Cos(randomAngle), 5 * Mathf.Sin(randomAngle)) + transform.position;
            Instantiate(weaponDrop, weaponPos, Quaternion.identity);
        }
    }

    public void RegainWeapon()
    {
        hasWeapon = true;
        playerShooting.EnableFire();
    }
}
