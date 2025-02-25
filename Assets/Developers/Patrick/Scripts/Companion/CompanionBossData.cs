using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CompanionBossData", menuName = "DataObject/CompanionBossData", order = 3)]
public class CompanionBossData : ScriptableObject
{
    [Header("Attacks")]
    public int leapsBeforeFeral;
    public float closeRangeDistance;

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


    [Header("Debug")]
    public bool drawGizmos;

}
