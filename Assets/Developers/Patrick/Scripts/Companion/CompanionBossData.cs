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

    [Header("Leap")]
    public float leapChargeTime;
    public float leapTravelTime;
    public float leapEndTime;
    public float leapTravelDistance;
    [Range(0.0f, 1.0f)] public float leapTargetTravelPercentage; // Where along the leap would the target be at the start of the leap as a percentage of the leap line
    public Sprite leapChargeSpriteLeft;
    public Sprite leapChargeSpriteRight;
    public Sprite leapMoveSprite;
    public Sprite leapEndSprite;

    [Header("Feral")]
    public int feralLeapAmount;
    public float feralLeapDelay;
    public float feralLeapEndTime;
    public float feralLeapAdditionalDistance;

    [Header("Spit")]
    public GameObject spitProjectile;
    public float spitProjectileLifespan;
    public float spitProjectileTravelDistance;
    public float spitChargeTime;
    public float spitSpawnDistance;
    public float spitSpawnAngle;
    public float spitEndTime;
    public Sprite spitChargeSprite;
    public Sprite spitAttackSprite;
    public Sprite spitEndSprite;

    [Header("Lick")]
    public GameObject lickProjectile;
    public float lickProjectileNumber;
    public float lickProjectileSpawnDistance;
    public float lickProjectileSeperationDistance;
    public float lickProjectileAngle;
    public float lickProjectileSpeed;
    public float lickChargeTime;
    public float lickEndTime;
    public Sprite lickChargeSprite;
    public Sprite lickAttackSprite;
    public Sprite lickEndSprite;

    [Header("Scream")]
    public GameObject screamProjectile;
    public float screamProjectileSpawnDistance;
    public int numberOfScreamProjectiles;
    public float screamProjectileSpeed;
    public float screamChargeTime;
    public float screamEndTime;
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
