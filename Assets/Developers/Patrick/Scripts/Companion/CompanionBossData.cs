using UnityEngine;

[CreateAssetMenu(fileName = "CompanionBossData", menuName = "DataObject/CompanionBossData", order = 3)]
public class CompanionBossData : ScriptableObject
{
    [Header("Attacks")]
    public float Health;
    public GameObject Healthbar;
    public int LeapsBeforeFeral;
    public float CloseRangeDistance;
    public float MoveSpeed;
    public float MoveSpeedMultiplier; // The multiplier that effects the speed the longer they move towards the player
    public LayerMask EnvironmentMask;
    public LayerMask PlayerMask;
    [Range(0f, 1f)] public float StageTwoHealthThreshold;
    [Range(0f, 1f)] public float StageThreeHealthThreshold;

    [Space(16)]
    [Header("Leap")]
    public float LeapDamage;
    public float LeapEndTime;
    [Range(0.0f, 1.0f)] public float LeapTargetTravelPercentage; // Where along the leap would the target be at the start of the leap as a percentage of the leap line
    [Header("Stage 1")]
    public float LeapChargeTimeStage1;
    public float LeapTravelTimeStage1;
    public float LeapTravelDistanceStage1;
    [Header("Stage 2")]
    public float LeapChargeTimeStage2;
    public float LeapTravelTimeStage2;
    public float LeapTravelDistanceStage2;
    [Header("Stage 3")]
    public float LeapChargeTimeStage3;
    public float LeapTravelTimeStage3;
    public float LeapTravelDistanceStage3;

    [Space(16)]
    [Header("Feral")]
    public float FeralLeapRestTime;
    [Header("Stage 1")]
    public int FeralLeapAmountStage1;
    public float FeralLeapDelayStage1;
    public float FeralLeapTravelTimeStage1;
    public float FeralLeapDistanceStage1;
    [Header("Stage 2")]
    public int FeralLeapAmountStage2;
    public float FeralLeapDelayStage2;
    public float FeralLeapTravelTimeStage2;
    public float FeralLeapDistanceStage2;
    [Header("Stage 3")]
    public int FeralLeapAmountStage3;
    public float FeralLeapDelayStage3;
    public float FeralLeapTravelTimeStage3;
    public float FeralLeapDistanceStage3;

    [Space(16)]
    [Header("Spit")]
    public GameObject SpitProjectile;
    public float SpitProjectileLifespan;
    public float SpitChargeTime;
    public float SpitSpawnDistance;
    public float SpitSpawnAngle;
    public float SpitEndTime;
    public float SpitProjectileDamage;
    [Header("Stage 1")]
    public float SpitProjectileTravelDistance1;
    public float SpitProjectileSize1;
    [Header("Stage 2")]
    public float SpitProjectileTravelDistance2;
    public float SpitProjectileSize2;
    [Header("Stage 3")]
    public float SpitProjectileTravelDistance3;
    public float SpitProjectileSize3;

    [Space(16)]
    [Header("Lick")]
    public GameObject LickProjectile;
    public float LickProjectileSpawnDistance;
    public float LickProjectileSeperationDistance;
    public float LickProjectileAngle;
    public float LickProjectileSize;
    public float LickProjectileDamage;
    public float LickChargeTime;
    public float LickEndTime;
    [Header("Stage 1")]
    public float LickProjectileSpeed1;
    public float LickWaveGapStage1;
    public int LickWavesStage1;
    public int LickProjectilesStage1;
    public int LickLastWaveProjectilesStage1;
    [Header("Stage 2")]
    public float LickProjectileSpeed2;
    public float LickWaveGapStage2;
    public int LickWavesStage2;
    public int LickProjectilesStage2;
    public int LickLastWaveProjectilesStage2;
    [Header("Stage 3")]
    public float LickProjectileSpeed3;
    public float LickWaveGapStage3;
    public int LickWavesStage3;
    public int LickProjectilesStage3;
    public int LickLastWaveProjectilesStage3;

    [Space(16)]
    [Header("Scream")]
    public GameObject ScreamProjectile;
    public float ScreamProjectileSize;
    public float ScreamProjectileDamage;
    public float ScreamProjectileSpawnDistance;
    public int NumberOfScreamProjectiles;
    public float ScreamChargeTime;
    public float ScreamEndTime;
    [Header("Stage 1")]
    public float ScreamProjectileSpeed1;
    public float ScreamWaveGapStage1;
    public int ScreamWavesStage1;
    [Header("Stage 2")]
    public float ScreamProjectileSpeed2;
    public float ScreamWaveGapStage2;
    public int ScreamWavesStage2;
    [Header("Stage 3")]
    public float ScreamProjectileSpeed3;
    public float ScreamWaveGapStage3;
    public int ScreamWavesStage3;

    [Header("Animation Timings")]
    public int IdleFrames;
    public int RunFrames;
    public int LickFrames;
    public int SpitFrames;
    public int ScreamStartFrames;
    public int ScreamContinuedFrames;
    public int LeapWindupFrames;
    public int LeapMovementFrames;

    [HideInInspector] public float IdleTiming;
    [HideInInspector] public float RunTiming;
    [HideInInspector] public float LickTiming;
    [HideInInspector] public float SpitTiming;
    [HideInInspector] public float ScreamStartTiming;
    [HideInInspector] public float ScreamContinuedTiming;
    [HideInInspector] public float LeapWindupTiming;
    [HideInInspector] public float LeapMovementTiming;

    [Header("Debug")]
    [Min(0.0f)] public float CompanionHeight;
    [Min(0.0f)] public float CompanionWidth;
    public float HeightOffset;
    public float WidthOffset;

    [Range(1, 3)] public int HeatUpStage;
    public bool DrawRange;
    public bool DrawLeap;
    public bool DrawFeralLeap;
    public bool DrawSpit;
    public bool DrawLick;
    public bool DrawScream;

    private void OnValidate()
    {
        IdleTiming = (float)IdleFrames / 30.0f;
        RunTiming = (float)RunFrames / 30.0f;
        LickTiming = (float)LickFrames / 30.0f;
        SpitTiming = (float)SpitFrames / 30.0f;
        ScreamStartTiming = (float)ScreamStartFrames / 30.0f;
        ScreamContinuedTiming = (float)ScreamContinuedFrames / 30.0f;
        LeapWindupTiming = (float)LeapWindupFrames / 30.0f;
        LeapMovementTiming = (float)LeapMovementFrames / 30.0f;
    }
}
