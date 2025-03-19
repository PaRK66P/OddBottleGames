using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "PlayerData", menuName = "DataObject/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    [Header("Health")]
    public float health = 100;
    public GameObject healthbar;

    [Header("Movement")]
    [Min(0.0f)] public float speed;
    [Range(0.0f, 1.0f)] public float acceleration;
    [HideInInspector] public float accelerationRate;
    [Range(0.0f, 1.0f)] public float deceleration;
    [HideInInspector] public float decelerationRate;

    [Header("Dashing")]
    [Min(0.0f)] public float dashTime;
    [Min(0.0f)] public float dashDistance;
    [Min(0.0f)] public float evolvedDashExtraDistance;
    [Min(0.0f)] public float dashCooldown;
    [Min(0)] public int numberOfDashCharges;
    [Min(0.0f)] public float dashRechargeTime;
    [Min(0.0f)] public float evolvedDashDamage;

    [Header("Collisions")]
    public LayerMask damageLayers;
    public LayerMask enemyLayers;

    [Header("Shooting")]
    public GameObject ammoUIPrefab;
    public GameObject ammoUIObject;
    public GameObject reloadUISlider;
    [Min(0.0f)] public float fireRate;
    [Min(0.0f)] public float maxTimeToChargeShot;
    [Min(0.0f)] public float minTimeToChargeShot;
    [Min(1)] public int shotsTillFullCharge;
    [Min(0.0f)] public float chargeShotIntervals;
    [Min(1)] public int maxAmmo;
    [Min(0.0f)] public float reloadTime;
    [Min(1.0f)] public float damageMultiplier;

    [Header("Projectile")]
    public GameObject baseProjectileType;
    [Min(0.0f)] public float baseProjectileSpeed;

    [Header("Damage")]
    [Min(0.0f)] public float controlLossTime;

    private void OnValidate()
    {
        accelerationRate = (50 * (acceleration * speed)) / speed;
        decelerationRate = (50 * (deceleration * speed)) / speed;
    }
}
