using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class CompanionAnimationHandler : MonoBehaviour
{
    public enum FacingDirection
    {
        Front,
        Back,
        Left,
        Right
    }

    // Animations
    [Header("Directions")]
    [SpineAnimation]
    public string FrontDirection;
    [SpineAnimation]
    public string BackDirection;
    [SpineAnimation]
    public string SideDirection;

    [Header("Animations")]

    // Components
    private SkeletonAnimation _skeletonAnimation;

    // Values
    public Spine.AnimationState SpineAnimationState;
    public Spine.Skeleton SpineSkeleton;

    private FacingDirection _currentDirection = FacingDirection.Front;

    // Start is called before the first frame update
    void Start()
    {
        _skeletonAnimation = GetComponent<SkeletonAnimation>();
        SpineAnimationState = _skeletonAnimation.AnimationState;
        SpineSkeleton = _skeletonAnimation.Skeleton;
    }
    public void UpdateFacingDirection(FacingDirection facingDirection)
    {
        if (facingDirection == _currentDirection) { return; }

        _currentDirection = facingDirection;

        switch (facingDirection)
        {
            case FacingDirection.Front:
                SpineSkeleton.ScaleX = 1.0f;
                SpineAnimationState.SetAnimation(0, FrontDirection, _skeletonAnimation);
                break;
            case FacingDirection.Back:
                SpineSkeleton.ScaleX = 1.0f;
                SpineAnimationState.SetAnimation(0, BackDirection, _skeletonAnimation);
                break;
            case FacingDirection.Left:
                SpineSkeleton.ScaleX = 1.0f;
                SpineAnimationState.SetAnimation(0, SideDirection, _skeletonAnimation);
                break;
            case FacingDirection.Right: // Right side animations are funky as the whole SpineSkeleton's scale is reversed
                SpineSkeleton.ScaleX = -1.0f;
                SpineAnimationState.SetAnimation(0, SideDirection, _skeletonAnimation);
                break;
        }
    }
}
