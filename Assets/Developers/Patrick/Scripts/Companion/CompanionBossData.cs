using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CompanionBossData", menuName = "DataObject/CompanionBossData", order = 3)]
public class CompanionBossData : ScriptableObject
{
    [Header("Attacks")]
    public float health;
    public int leapsBeforeFeral;
    public float closeRangeDistance;
    public LayerMask environmentMask;
    public LayerMask playerMask;

    [Header("Leap")]
    public float leapChargeTime;
    public float leapTravelTime;
    public float leapEndTime;
    public float leapTravelDistance;

    [Header("Feral")]
    public int feralLeapAmount;
    public float feralLeapDelay;
    public float feralLeapEndTime;

    [Header("Spit")]
    public GameObject spitProjectile;
    public float spitProjectileLifespan;
    public float spitProjectileTravelDistance;
    public float spitChargeTime;
    public float spitSpawnDistance;
    public float spitSpawnAngle;
    public float spitEndTime;

    [Header("Lick")]
    public GameObject lickProjectile;
    public float lickProjectileNumber;
    public float lickProjectileSpawnDistance;
    public float lickProjectileSeperationDistance;
    public float lickProjectileAngle;
    public float lickProjectileSpeed;
    public float lickChargeTime;
    public float lickEndTime;

    [Header("Scream")]
    public GameObject screamProjectile;
    public float screamProjectileSpawnDistance;
    public int numberOfScreamProjectiles;
    public float screamProjectileSpeed;
    public float screamChargeTime;
    public float screamEndTime;


    [Header("Debug")]
    public bool drawRange;
    public bool drawLeaps;
    public bool drawSpit;
    public bool drawLick;
    public bool drawScream;

}
