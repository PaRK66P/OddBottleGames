using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FriendData", menuName = "DataObject/CompanionFriendData", order = 4)]
public class CompanionFriendData : ScriptableObject
{
    public LayerMask environmentLayer;
    public LayerMask enemyLayer;
    public float leapTravelTime;
    public float leapChargeTime;
    public float leapDistance;

    public float leapDamage;

    public float idleSpeed;
}
