using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class LilGuysAnimationHandler : MonoBehaviour
{
    [Header("Animations")]
    [SpineAnimation]
    public string IdleAnimation;
    [SpineAnimation]
    public string AttackAnimation;
    [SpineAnimation]
    public string DeathAnimation;
    [SpineAnimation]
    public string HurtAnimation;

    // Components
    private SkeletonAnimation _skeletonAnimation;

    // Values
    public Spine.AnimationState SpineAnimationState;
    public Spine.Skeleton SpineSkeleton;

    private bool _isIdleInTrack;
    private bool _isDead;

    // Start is called before the first frame update
    void Start()
    {
        _skeletonAnimation = GetComponent<SkeletonAnimation>();
        SpineAnimationState = _skeletonAnimation.AnimationState;
        SpineSkeleton = _skeletonAnimation.Skeleton;

        SpineAnimationState.SetAnimation(0, IdleAnimation, true);
        _isIdleInTrack = true;
        _isDead = false;
    }

    public void AddIdle()
    {
        if (_isIdleInTrack || _isDead) { return; }
        _isIdleInTrack = true;

        SpineAnimationState.AddAnimation(0, IdleAnimation, true, 0.0f);
    }

    public void SetAttack()
    {
        if(_isDead) { return; }
        _isIdleInTrack = false;

        SpineAnimationState.SetAnimation(0, AttackAnimation, false);
    }

    public void SetDeath()
    {
        _isDead = true;

        SpineAnimationState.SetAnimation(0, DeathAnimation, false);
    }

    public void SetHurt()
    {
        if (_isDead) { return; }
        _isIdleInTrack = false;

        SpineAnimationState.SetAnimation(0, HurtAnimation, false);
    }
    
}
