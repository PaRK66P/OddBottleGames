using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDebugData", menuName = "DataObject/PlayerDebugData", order = 2)]
public class PlayerDebugData : ScriptableObject
{
    [Header("Input")]
    [Min(0.0f)] public float DashInputBuffer;
    [Min(0.0f)] public float FiringInputBuffer;

    [Header("Dashing")]
    public bool CanDashTowardsMouse = false;

    [Header("Charging")]
    public bool IsSlowedOnCharge = false;
    public bool IsUsingConstantChargeTime = false;
    public float ConstantChargeTimeValue = 0.15f;
}
