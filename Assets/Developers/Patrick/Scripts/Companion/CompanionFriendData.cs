using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FriendData", menuName = "DataObject/CompanionFriendData", order = 4)]
public class CompanionFriendData : ScriptableObject
{
    [Header("Collisions")]
    public LayerMask environmentLayer;
    public LayerMask enemyLayer;
    public float detectionRadius;

    [Header("Idle")]
    public float idleSpeed;
    public float idleDistance;

    [Header("Leap")]
    public float leapTravelTime;
    public float leapChargeTime;
    public float leapDistance;

    public float leapDamage;
    public float leapEndTime;

    [Header("Recharge Zone")]
    public float rechargeZoneRadius;

    [Header("Gizmos")]
    public bool drawDetectionRange;
    public bool drawRechargeZone;
    public bool drawIdleDistance;
}
