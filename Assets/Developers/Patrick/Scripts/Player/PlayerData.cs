using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "PlayerData", menuName = "DataObject/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    [Header("Health")]
    public float Health = 100;
    public GameObject Healthbar;
    [Range(0.0f, 1.0f)] public float DamageImpactFrameScale;
    [Min(0.0f)] public float DamageImpactFrameDuration;

    [Header("Movement")]
    [Min(0.0f)] public float Speed;
    [Range(0.0f, 1.0f)] public float Acceleration;
    [HideInInspector] public float AccelerationRate;
    [Range(0.0f, 1.0f)] public float Deceleration;
    [HideInInspector] public float DecelerationRate;

    [Header("Dashing")]
    [Min(0.0f)] public float DashTime;
    [Min(0.0f)] public float DashDistance;
    [Min(0.0f)] public float EvolvedDashExtraDistance;
    [Min(0.0f)] public float DashCooldown;
    [Min(0)] public int NumberOfDashCharges;
    [Min(0.0f)] public float DashRechargeTime;
    [Min(0.0f)] public float EvolvedDashDamage;

    [Header("Collisions")]
    public LayerMask DamageLayers;
    public LayerMask EnemyLayers;

    [Header("Shooting")]
    public GameObject AmmoUIPrefab;
    public GameObject AmmoUIObject;
    public GameObject ReloadUISlider;
    [Min(0.0f)] public float FireRate;
    [Min(0.0f)] public float MaxTimeToChargeShot;
    [Min(0.0f)] public float MinTimeToChargeShot;
    [Min(1)] public int ShotsTillFullCharge;
    [Min(0.0f)] public float ChargeShotIntervals;
    [Min(1)] public int MaxAmmo;
    [Min(0.0f)] public float ReloadTime;
    [Min(1.0f)] public float DamageMultiplier;
    [Range(0.0f, 1.0f)] public float ChargeSlowDown;
    [Range(0.0f, 1.0f)] public float MaxChargeShotImpactSlowDown;
    [Min(0.0f)] public float ChargeShotImpactFrameDuration;

    [Header("Projectile")]
    public GameObject BaseProjectileType;
    [Min(0.0f)] public float BaseProjectileSpeed;

    [Header("Damage")]
    [Min(0.0f)] public float ControlLossTime;

    private void OnValidate()
    {
        AccelerationRate = (50 * (Acceleration * Speed)) / Speed;
        DecelerationRate = (50 * (Deceleration * Speed)) / Speed;
    }
}
