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

    public enum ActionState
    {
        None,
        Idle,
        Run,
        Lick,
        Spit,
        Scream,
        ScreamContinue,
        Windup,
        Leap
    }

    // Animations
    [Header("Directions")]
    [SpineAnimation]
    public string FrontDirection;
    [SpineAnimation]
    public string BackDirection;
    [SpineAnimation]
    public string SideDirection;

    [Header("Cloak")]
    [SpineAnimation]
    public string CloakLeft;
    [SpineAnimation]
    public string CloakRight;

    [Header("Animations")]
    [SpineAnimation]
    public string IdleAnimation;
    [SpineAnimation]
    public string RunAnimation;
    [SpineAnimation]
    public string LickAnimation;
    [SpineAnimation]
    public string SpitAnimation;
    [SpineAnimation]
    public string ScreamStartAnimation;
    [SpineAnimation]
    public string ScreamContinuedAnimation;
    [SpineAnimation]
    public string LeapWindupAnimation;
    [SpineAnimation]
    public string LeapMovementAnimation;
    [SpineAnimation]
    public string HurtAnimation;

    // Components
    private SkeletonAnimation _skeletonAnimation;
    private CompanionManager _companionManager;

    // Values
    public Spine.AnimationState SpineAnimationState;
    public Spine.Skeleton SpineSkeleton;

    private FacingDirection _currentDirection = FacingDirection.Front;
    private ActionState _currentAction = ActionState.Idle;

    private Spine.TrackEntry _animationTrack;

    // Start is called before the first frame update
    void Start()
    {
        _skeletonAnimation = GetComponent<SkeletonAnimation>();
        SpineAnimationState = _skeletonAnimation.AnimationState;
        SpineSkeleton = _skeletonAnimation.Skeleton;

        SpineAnimationState.SetAnimation(0, FrontDirection, _skeletonAnimation);
        SpineAnimationState.SetAnimation(1, IdleAnimation, true);
    }

    public void SetManager(ref CompanionManager manager)
    {
        _companionManager = manager;
    }

    public void ResetActionState()
    {
        _currentAction = ActionState.None;
    }

    public void UpdateFacingDirection(FacingDirection facingDirection)
    {
        if (facingDirection == _currentDirection) { return; }
        if (_currentAction == ActionState.Windup) { return; }

        _currentDirection = facingDirection;

        switch (facingDirection)
        {
            case FacingDirection.Front:
                SpineSkeleton.ScaleX = 1.0f;
                SpineAnimationState.SetAnimation(0, FrontDirection, _skeletonAnimation);
                SpineAnimationState.SetAnimation(2, CloakLeft, _skeletonAnimation);
                break;
            case FacingDirection.Back:
                SpineSkeleton.ScaleX = 1.0f;
                SpineAnimationState.SetAnimation(0, BackDirection, _skeletonAnimation);
                SpineAnimationState.SetAnimation(2, CloakRight, _skeletonAnimation);
                break;
            case FacingDirection.Left:
                SpineSkeleton.ScaleX = 1.0f;
                SpineAnimationState.SetAnimation(0, SideDirection, _skeletonAnimation);
                SpineAnimationState.SetAnimation(2, CloakLeft, _skeletonAnimation);
                break;
            case FacingDirection.Right: // Right side animations are funky as the whole SpineSkeleton's scale is reversed
                SpineSkeleton.ScaleX = -1.0f;
                SpineAnimationState.SetAnimation(0, SideDirection, _skeletonAnimation);
                SpineAnimationState.SetAnimation(2, CloakLeft, _skeletonAnimation);
                break;
        }
    }

    public void SetToIdleAnimation()
    {
        if(_currentAction == ActionState.Idle) { return; }
        _currentAction = ActionState.Idle;
        _animationTrack = SpineAnimationState.SetAnimation(1, IdleAnimation, true);
    }

    public void SetToRunAnimation()
    {
        if (_currentAction == ActionState.Run) { return; }
        _currentAction = ActionState.Run;
        _animationTrack = SpineAnimationState.SetAnimation(1, RunAnimation, true);
    }

    public void SetToLickAnimation()
    {
        if (_currentAction == ActionState.Lick) { return; }
        _currentAction = ActionState.Lick;
        _animationTrack = SpineAnimationState.SetAnimation(1, LickAnimation, false);
    }

    public void SetToSpitAnimation()
    {
        if (_currentAction == ActionState.Spit) { return; }
        _currentAction = ActionState.Spit;
        _animationTrack = SpineAnimationState.SetAnimation(1, SpitAnimation, false);
    }

    public void StartScream()
    {
        if (_currentAction == ActionState.Scream) { return; }
        _currentAction = ActionState.Scream;
        _animationTrack = SpineAnimationState.SetAnimation(1, ScreamStartAnimation, false);
    }

    public void ContinueScream()
    {
        if(_currentAction == ActionState.ScreamContinue) { return; }
        _currentAction = ActionState.ScreamContinue;
        _animationTrack = SpineAnimationState.SetAnimation(1, ScreamContinuedAnimation, false);
    }

    public void StartLeap()
    {
        if (_currentAction == ActionState.Windup) { return; }
        _currentAction = ActionState.Windup;
        
        _currentDirection = FacingDirection.Front;
        SpineSkeleton.ScaleX = _companionManager.IsPlayerOnRightSide()?1.0f:-1.0f;
        SpineAnimationState.SetAnimation(0, FrontDirection, _skeletonAnimation);
        SpineAnimationState.SetAnimation(2, CloakLeft, _skeletonAnimation);

        _animationTrack = SpineAnimationState.SetAnimation(1, LeapWindupAnimation, false);
    }

    public void PlayLeapMovement()
    {
        if (_currentAction == ActionState.Leap) { return; }
        _currentAction = ActionState.Leap;
        _animationTrack = SpineAnimationState.SetAnimation(1, LeapMovementAnimation, false);
    }

    public void ChangeAnimationTrackSpeed(float speed)
    {
        if (_animationTrack != null)
        {
            _animationTrack.TimeScale = speed;
        }
    }

    public void ResetAnimationTrackSpeed()
    {
        if (_animationTrack != null)
        {
            _animationTrack.TimeScale = 1.0f;
        }
    }

    public void AddHurtAnimation()
    {
        SpineAnimationState.SetAnimation(3, HurtAnimation, false);
        SpineAnimationState.AddEmptyAnimation(3, 0, 0);
    }
}
