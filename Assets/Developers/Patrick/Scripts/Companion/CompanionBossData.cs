using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CompanionBossData", menuName = "DataObject/CompanionBossData", order = 3)]
public class CompanionBossData : ScriptableObject
{
    [Header("Attacks")]
    public float health;
    public GameObject healthbar;
    public int leapsBeforeFeral;
    public float closeRangeDistance;
    public float moveSpeed;
    public float moveSpeedMultiplier; // The multiplier that effects the speed the longer they move towards the player
    public LayerMask environmentMask;
    public LayerMask playerMask;
    public Sprite idleSprite;
    [Range(0f, 1f)] public float stageTwoHealthThreshold;
    [Range(0f, 1f)] public float stageThreeHealthThreshold;

    [Space(16)]
    [Header("Leap")]
    public float leapDamage;
    public float leapEndTime;
    [Range(0.0f, 1.0f)] public float leapTargetTravelPercentage; // Where along the leap would the target be at the start of the leap as a percentage of the leap line
    [Header("Stage 1")]
    public float leapChargeTimeStage1;
    public float leapTravelTimeStage1;
    public float leapTravelDistanceStage1;
    [Header("Stage 2")]
    public float leapChargeTimeStage2;
    public float leapTravelTimeStage2;
    public float leapTravelDistanceStage2;
    [Header("Stage 3")]
    public float leapChargeTimeStage3;
    public float leapTravelTimeStage3;
    public float leapTravelDistanceStage3;
    [Header("Animations")]
    public Sprite leapChargeSpriteLeft;
    public Sprite leapChargeSpriteRight;
    public Sprite leapMoveSprite;
    public Sprite leapEndSprite;

    [Space(16)]
    [Header("Feral")]
    public float feralLeapRestTime;
    [Header("Stage 1")]
    public int feralLeapAmountStage1;
    public float feralLeapDelayStage1;
    public float feralLeapTravelTimeStage1;
    public float feralLeapDistanceStage1;
    [Header("Stage 2")]
    public int feralLeapAmountStage2;
    public float feralLeapDelayStage2;
    public float feralLeapTravelTimeStage2;
    public float feralLeapDistanceStage2;
    [Header("Stage 3")]
    public int feralLeapAmountStage3;
    public float feralLeapDelayStage3;
    public float feralLeapTravelTimeStage3;
    public float feralLeapDistanceStage3;

    [Space(16)]
    [Header("Spit")]
    public GameObject spitProjectile;
    public float spitProjectileLifespan;
    public float spitChargeTime;
    public float spitSpawnDistance;
    public float spitSpawnAngle;
    public float spitEndTime;
    public float spitProjectileDamage;
    [Header("Stage 1")]
    public float spitProjectileTravelDistance1;
    public float spitProjectileSize1;
    [Header("Stage 2")]
    public float spitProjectileTravelDistance2;
    public float spitProjectileSize2;
    [Header("Stage 3")]
    public float spitProjectileTravelDistance3;
    public float spitProjectileSize3;
    [Header("Animations")]
    public Sprite spitChargeSprite;
    public Sprite spitAttackSprite;
    public Sprite spitEndSprite;

    [Space(16)]
    [Header("Lick")]
    public GameObject lickProjectile;
    public float lickProjectileSpawnDistance;
    public float lickProjectileSeperationDistance;
    public float lickProjectileAngle;
    public float lickProjectileSize;
    public float lickProjectileDamage;
    public float lickChargeTime;
    public float lickEndTime;
    [Header("Stage 1")]
    public float lickProjectileSpeed1;
    public float lickWaveGapStage1;
    public int lickWavesStage1;
    public int lickProjectilesStage1;
    public int lickLastWaveProjectilesStage1;
    [Header("Stage 2")]
    public float lickProjectileSpeed2;
    public float lickWaveGapStage2;
    public int lickWavesStage2;
    public int lickProjectilesStage2;
    public int lickLastWaveProjectilesStage2;
    [Header("Stage 3")]
    public float lickProjectileSpeed3;
    public float lickWaveGapStage3;
    public int lickWavesStage3;
    public int lickProjectilesStage3;
    public int lickLastWaveProjectilesStage3;
    [Header("Animations")]
    public Sprite lickChargeSprite;
    public Sprite lickAttackSprite;
    public Sprite lickEndSprite;

    [Space(16)]
    [Header("Scream")]
    public GameObject screamProjectile;
    public float screamProjectileSize;
    public float screamProjectileDamage;
    public float screamProjectileSpawnDistance;
    public int numberOfScreamProjectiles;
    public float screamChargeTime;
    public float screamEndTime;
    [Header("Stage 1")]
    public float screamProjectileSpeed1;
    public float screamWaveGapStage1;
    public int screamWavesStage1;
    [Header("Stage 2")]
    public float screamProjectileSpeed2;
    public float screamWaveGapStage2;
    public int screamWavesStage2;
    [Header("Stage 3")]
    public float screamProjectileSpeed3;
    public float screamWaveGapStage3;
    public int screamWavesStage3;
    [Header("Animations")]
    public Sprite screamChargeSprite;
    public Sprite screamAttackSprite;
    public Sprite screamEndSprite;


    [Header("Debug")]
    public bool drawRange;
    public bool drawLeaps;
    public bool drawSpit;
    public bool drawLick;
    public bool drawScream;

    public Sprite GetSprite(CompanionAnimations.AnimationState animation, bool isFacingRight)
    {
        switch (animation)
        {
            case CompanionAnimations.AnimationState.IDLE:
                return idleSprite;

            case CompanionAnimations.AnimationState.LEAP_CHARGE:
                if (isFacingRight)
                {
                    return leapChargeSpriteRight;
                }
                return leapChargeSpriteLeft;

            case CompanionAnimations.AnimationState.LEAP_MOVING:
                return leapMoveSprite;

            case CompanionAnimations.AnimationState.LEAP_END:
                return leapEndSprite;

            case CompanionAnimations.AnimationState.SPIT_CHARGE:
                return spitChargeSprite;

            case CompanionAnimations.AnimationState.SPIT_ATTACK:
                return spitAttackSprite;

            case CompanionAnimations.AnimationState.SPIT_END:
                return spitEndSprite;

            case CompanionAnimations.AnimationState.LICK_CHARGE:
                return lickChargeSprite;

            case CompanionAnimations.AnimationState.LICK_ATTACK:
                return lickAttackSprite;

            case CompanionAnimations.AnimationState.LICK_END:
                return lickEndSprite;

            case CompanionAnimations.AnimationState.SCREAM_CHARGE:
                return screamChargeSprite;

            case CompanionAnimations.AnimationState.SCREAM_ATTACK:
                return screamAttackSprite;

            case CompanionAnimations.AnimationState.SCREAM_END:
                return screamEndSprite;

            default:
                return null;
        }
    }
}
