using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CompanionBossData", menuName = "DataObject/CompanionBossData", order = 3)]
public class CompanionBossData : ScriptableObject
{
    [Header("Attacks")]
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

    [Header("Scream")]
    public GameObject screamProjectile;
    public float screamProjectileSpawnDistance;
    public int numberOfScreamProjectiles;
    public float screamProjectileSpeed;


    [Header("Debug")]
    public bool drawLeaps;
    public bool drawSpit;
    public bool drawLick;
    public bool drawScream;

}
