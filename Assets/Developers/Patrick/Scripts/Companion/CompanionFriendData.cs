using UnityEngine;

[CreateAssetMenu(fileName = "FriendData", menuName = "DataObject/CompanionFriendData", order = 4)]
public class CompanionFriendData : ScriptableObject
{
    [Header("Collisions")]
    public LayerMask EnvironmentLayer;
    public LayerMask EnemyLayer;
    public LayerMask PlayerAttacksLayer;
    public float DetectionRadius;

    [Header("Idle")]
    public float IdleSpeed;
    public float IdleDistance;

    [Header("Attack")]
    public float MoveSpeed;

    [Header("Leap")]
    public float LeapTravelTime;
    public float LeapChargeTime;
    public float LeapDistance;
    [Range(0.0f, 1.0f)] public float LeapTargetTravelPercentage; // Where along the leap would the target be at the start of the leap as a percentage of the leap line

    public float LeapDamage;
    public float LeapEndTime;

    [Header("Gizmos")]
    public bool DrawDetectionRange;
    public bool DrawIdleDistance;
}
