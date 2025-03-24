using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class PlayerAnimationHandler : MonoBehaviour
{
    public enum FacingDirection
    {
        FRONT,
        BACK,
        LEFT,
        RIGHT
    }


    [Header("Directions")]
    [SpineAnimation]
    public string frontDirection;
    [SpineAnimation]
    public string backDirection;
    [SpineAnimation]
    public string sideDirection;

    [Header("Animations")]
    [SpineAnimation]
    public string runAnimation;
    [SpineAnimation]
    public string dashAnimation;

    [Header("Transitions")]
    [SpineAnimation]
    public string turnLeftTransition;
    [SpineAnimation]
    public string turnRightTransition;

    private SkeletonAnimation skeletonAnimation;

    public Spine.AnimationState spineAnimationState;
    public Spine.Skeleton skeleton;

    private FacingDirection currentDirection = FacingDirection.FRONT;

    private bool _isMoving = false;
    private bool _isDashing = false;

    // Start is called before the first frame update
    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        spineAnimationState = skeletonAnimation.AnimationState;
        skeleton = skeletonAnimation.Skeleton;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateFacingDirection(FacingDirection facingDirection)
    {
        if (facingDirection == currentDirection) { return; }

        switch (facingDirection)
        {
            case FacingDirection.FRONT:
                spineAnimationState.SetAnimation(0, frontDirection, skeletonAnimation);
                break;
            case FacingDirection.BACK:
                spineAnimationState.SetAnimation(0, backDirection, skeletonAnimation);
                break;
            case FacingDirection.LEFT:
            case FacingDirection.RIGHT:
                spineAnimationState.SetAnimation(0, sideDirection, skeletonAnimation);
                break;
        }

        skeleton.ScaleX = (facingDirection == FacingDirection.RIGHT) ? -1.0f : 1.0f;

        currentDirection = facingDirection;
    }

    public void StartDashAnimation()
    {
        spineAnimationState.SetAnimation(1, dashAnimation, false);
    }

    public void EndDashAnimation()
    {
        spineAnimationState.SetEmptyAnimation(1, 0.0f);

        if (_isMoving)
        {
            spineAnimationState.SetAnimation(1, runAnimation, true);
        }
        
    }

    public void UpdateMovementAnimation(bool isMoving)
    {
        if(_isMoving == isMoving) { return; }

        _isMoving = isMoving;

        if (_isDashing) {  return; }

        spineAnimationState.SetEmptyAnimation(1, 0.0f);

        if (_isMoving)
        {
            spineAnimationState.SetAnimation(1, runAnimation, true);
        }
    }
}
