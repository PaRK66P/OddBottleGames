using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CompanionBossData", menuName = "DataObject/CompanionBossData", order = 3)]
public class CompanionBossData : ScriptableObject
{
    public int leapsBeforeFeral;
    public float closeRangeDistance;

    public float leapChargeTime;
    public float leapTravelTime;
    public float leapEndTime;
    public float leapTravelDistance;

    public int feralLeapAmount;
    public float feralLeapDelay;

    public GameObject spitProjectile;
    public float spitChargeTime;
    public float spitSpawnDistance;
    public float spitSpawnAngle;

    public bool drawGizmos;

}
