using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private ObjectPoolManager poolManager;

    [SerializeField]
    private PlayerData playerData;
    [SerializeField]
    private PlayerDebugData debugData;

    private PlayerInputManager playerInputManager;
    private PlayerMovement playerMovement;
    private PlayerShooting playerShooting;

    private GameObject weaponDrop;

    private bool hasWeapon = true;
    private bool isDamaged = false;

    private float timeOfDamage = -10.0f;
    private float invulnerableTime = 1.0f;

    private event EventHandler OnDamageTaken;

    private Rigidbody2D rb;
    private SpriteRenderer image;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        image = rb.GetComponent<SpriteRenderer>();

        playerInputManager = gameObject.AddComponent<PlayerInputManager>();
        playerMovement = gameObject.AddComponent<PlayerMovement>();
        playerShooting = gameObject.AddComponent<PlayerShooting>();

        playerInputManager.InitialiseComponent(ref playerMovement, ref playerShooting);
        playerMovement.InitialiseComponent(playerData.speed, playerData.dashTime, playerData.dashDistance, playerData.dashCooldown, playerData.dashInputBuffer, playerData.damageLayers);
        playerShooting.InitialiseComponent(playerData.baseProjectileType, playerData.baseProjectileSpeed, debugData.canDropWeapon, ref poolManager);

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
        if (isDamaged)
        {
            if(Time.time - timeOfDamage >= playerData.controlLossTime) // Exceeded the time for player to not have control
            {
                playerInputManager.EnableInput();
            }
            if(Time.time - timeOfDamage > invulnerableTime) // We've exceeded the time for being invulnerable
            {
                rb.excludeLayers = 0;
                isDamaged = false;
                image.color = Color.white;
            }
        }
    }

    public void TakeDamage(Vector2 damageDirection, float damageTime = 1.0f, float knockbackScalar = 1.0f)
    {
        if (isDamaged) { return; }

        rb.excludeLayers = playerData.damageLayers;
        isDamaged = true;
        playerInputManager.DisableInput();
        image.color = Color.blue;
        timeOfDamage = Time.time;
        invulnerableTime = damageTime;
        playerMovement.KnockbackPlayer(damageDirection, knockbackScalar);

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
