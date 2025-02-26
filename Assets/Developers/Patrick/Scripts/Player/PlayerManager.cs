using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private ObjectPoolManager poolManager;
    [SerializeField]
    private GameObject UICanvas;
    [SerializeField]
    private GameObject PlayerCanvas;

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

    private int health = 100;
    private GameObject healthbar;

    private event EventHandler OnDamageTaken;

    private Rigidbody2D rb;
    private SpriteRenderer image;

    private CanvasGroup canvGroup;
    [SerializeField]
    private bool fadeIn = false;
    [SerializeField]
    private bool fadeOut = false;
    [SerializeField]
    private float fadeTextTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        image = GetComponentInChildren<SpriteRenderer>();

        playerInputManager = gameObject.AddComponent<PlayerInputManager>();
        playerMovement = gameObject.AddComponent<PlayerMovement>();
        playerShooting = gameObject.AddComponent<PlayerShooting>();
        //movement component
        playerMovement.InitialiseComponent(ref playerData, ref debugData, ref UICanvas);
        //shooting component
        playerShooting.InitialiseComponent(playerData.ammoUIObject, playerData.fireRate, playerData.maxTimeToChargeShot, 
            playerData.minTimeToChargeShot, playerData.shotsTillFullCharge, playerData.chargeShotIntervals,
            playerData.maxAmmo, playerData.reloadTime, playerData.baseProjectileType, playerData.baseProjectileSpeed, 
            debugData.firingInputBuffer, debugData.canDropWeapon, ref poolManager, ref PlayerCanvas, playerData.reloadUISlider, 
            playerData.damageMultiplier);
        playerInputManager.InitialiseComponent(ref playerMovement, ref playerShooting);

        playerInputManager.EnableInput();

        if (debugData.canDropWeapon)
        {
            OnDamageTaken += DropWeapon;
        }


        healthbar = Instantiate(playerData.healthbar, UICanvas.transform.Find("PlayerUI"));
        healthbar.GetComponent<Slider>().maxValue = playerData.health;
        health = playerData.health;

        canvGroup = gameObject.transform.Find("PlayerCanvas").transform.Find("FadeInOutGroup").GetComponent<CanvasGroup>();

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

        healthbar.GetComponent<Slider>().value = health;


        if (fadeIn)
        {
            canvGroup.alpha += Time.deltaTime;
            if (canvGroup.alpha >= 1.0f)
            {
                fadeIn = false;
            }
        }
        if (fadeOut && !fadeIn)
        {
            canvGroup.alpha -= Time.deltaTime;
            if (canvGroup.alpha <= 0.0f)
            {
                fadeOut = false;
            }
        }
        if (canvGroup.alpha >= 1.0f)
        {
            fadeTextTimer += Time.deltaTime;
        }

        if (fadeTextTimer > 3.0f)
        {
            fadeOut = true;
        }

    }

    public void TakeDamage(Vector2 damageDirection, float damageTime = 1.0f, float knockbackScalar = 1.0f, int ammount = 20)
    {
        if (isDamaged) { return; }

        rb.excludeLayers = playerData.damageLayers;
        isDamaged = true;
        playerInputManager.DisableInput();
        image.color = Color.blue;
        timeOfDamage = Time.time;
        invulnerableTime = damageTime;
        playerMovement.KnockbackPlayer(damageDirection, knockbackScalar);
        playerShooting.InterruptFiring();
        fadeOut = true;

        OnDamageTaken?.Invoke(this, EventArgs.Empty);

        health -= ammount;
        if(health <= 1)
        {
            health = 1;
        }
    }

    //public void TakeDamage(Vector2 damageDirection, float damageTime = 1.0f, float knockbackScalar = 1.0f)
    //{
    //    if (isDamaged) { return; }

    //    rb.excludeLayers = playerData.damageLayers;
    //    isDamaged = true;
    //    playerInputManager.DisableInput();
    //    image.color = Color.blue;
    //    timeOfDamage = Time.time;
    //    invulnerableTime = damageTime;
    //    playerMovement.KnockbackPlayer(damageDirection, knockbackScalar);

    //    OnDamageTaken?.Invoke(this, EventArgs.Empty);

        
    //}

    public void Heal(int ammount)
    {
        health += ammount;
        if(health >= playerData.health)
        {
            health = playerData.health;
        }

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

    public void DisableInput()
    {
        playerInputManager.DisableInput();
    }

    public void EnableInput()
    {
        playerInputManager.EnableInput();
    }

    public void StartFadeInSpeech(string text)
    {
        fadeIn = true;
        fadeTextTimer = 0.0f;
        canvGroup.gameObject.GetComponentInChildren<TMP_Text>().text = text;
    }

    public void ChangeDashToggle(bool toggle)
    {
        debugData.dashTowardsMouse = toggle;
    }
}
