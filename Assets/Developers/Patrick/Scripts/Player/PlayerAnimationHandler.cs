using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class PlayerAnimationHandler : MonoBehaviour
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
    [SpineAnimation]
    public string RunAnimation;
    [SpineAnimation]
    public string DashAnimation;
    [SpineAnimation]
    public string DamageAnimation;

    [Header("Gun Arms")]
    [SpineAnimation]
    public string LeftSide;
    [SpineAnimation]
    public string RightSide;

    // Components
    private SkeletonAnimation _skeletonAnimation;

    // Values
    public Spine.AnimationState SpineAnimationState;
    public Spine.Skeleton SpineSkeleton;

    private FacingDirection _currentDirection = FacingDirection.Front;

    private bool _isMoving = false;
    private bool _isTakingDamage = false;
    private bool _isDashing = false;

    private Spine.TrackEntry _gunArmTrack;
    private Spine.TrackEntry _movementTrack;
    private float _currentArmRotationAngleFromRight;
    private float _currentMovementScale = 1.0f;
    private bool _isUsingLeftSide = true;

    // Start is called before the first frame update
    void Start()
    {
        _skeletonAnimation = GetComponent<SkeletonAnimation>();
        SpineAnimationState = _skeletonAnimation.AnimationState;
        SpineSkeleton = _skeletonAnimation.Skeleton;

        _gunArmTrack = SpineAnimationState.SetAnimation(2, LeftSide, true);
        _gunArmTrack.TimeScale = 0.0f; // Gun arm is a frame of the arm animation
    }

    // Changes player facing direction to the passed through direction
    public void UpdateFacingDirection(FacingDirection facingDirection)
    {
        if (facingDirection == _currentDirection) { return; }

        _currentDirection = facingDirection;

        if (_isTakingDamage) { return; }

        switch (facingDirection)
        {
            case FacingDirection.Front:
                SpineSkeleton.ScaleX = 1.0f;
                SetArmSide(true);
                SpineAnimationState.SetAnimation(0, FrontDirection, _skeletonAnimation);
                break;
            case FacingDirection.Back:
                SpineSkeleton.ScaleX = 1.0f;
                SetArmSide(false);
                SpineAnimationState.SetAnimation(0, BackDirection, _skeletonAnimation);
                break;
            case FacingDirection.Left:
                SpineSkeleton.ScaleX = 1.0f;
                SetArmSide(true);
                SpineAnimationState.SetAnimation(0, SideDirection, _skeletonAnimation);
                break;
            case FacingDirection.Right: // Right side animations are funky as the whole SpineSkeleton's scale is reversed
                SpineSkeleton.ScaleX = -1.0f;
                SetArmSide(true);
                SpineAnimationState.SetAnimation(0, SideDirection, _skeletonAnimation);
                break;
        }

        UpdateAimDirection();
    }

    public void StartDashAnimation()
    {
        SpineAnimationState.SetAnimation(1, DashAnimation, false);
    }

    public void EndDashAnimation()
    {
        if (_isTakingDamage) { return;} // Don't change values as currently in damage state
        if (_isMoving)
        {
            _movementTrack = SpineAnimationState.SetAnimation(1, RunAnimation, true);
            _movementTrack.TimeScale = _currentMovementScale;
        }
        else
        {
            SpineAnimationState.SetEmptyAnimation(1, 0.0f);
        }
    }

    public void UpdateMovementAnimation(bool isMoving)
    {
        if(_isMoving == isMoving || _isTakingDamage) { return; }

        _isMoving = isMoving;

        if (_isDashing) {  return; } // Values shouldn't change as would mess with the dash animations

        if (_isMoving)
        {
            _movementTrack = SpineAnimationState.SetAnimation(1, RunAnimation, true);
            _movementTrack.TimeScale = _currentMovementScale;
        }
        else
        {
            SpineAnimationState.SetEmptyAnimation(1, 0.0f);
        }
    }

    // Changes frequency of movement speed
    public void UpdateMovementScale(float scale)
    {
        _currentMovementScale = scale;

        if (_isMoving)
        {
            _movementTrack.TimeScale = scale;
        }
    }

    private void SetArmSide(bool isLeftSide)
    {
        if (isLeftSide == _isUsingLeftSide) { return; }
        _isUsingLeftSide = isLeftSide;

        if (_isUsingLeftSide)
        {
            _gunArmTrack = SpineAnimationState.SetAnimation(2, LeftSide, true);
        }
        else
        {
            _gunArmTrack = SpineAnimationState.SetAnimation(2, RightSide, true);
        }

        _gunArmTrack.TimeScale = 0.0f;

    }

    public void SetAimDirection(Vector2 direction)
    {
        if (_currentDirection == FacingDirection.Right) { direction.x *= -1.0f; } // Scale is reversed when facing right

        float AngleFromRight = Vector3.SignedAngle(Vector3.right, new Vector3(direction.x, direction.y, Vector3.right.z), new Vector3(0.0f, 0.0f, 1.0f));

        if (Mathf.Sign(AngleFromRight) == -1.0f) // Prevent angle being negative
        {
            AngleFromRight += 360.0f;
        }

        _currentArmRotationAngleFromRight = AngleFromRight;
        UpdateAimDirection();
    }

    private void UpdateAimDirection()
    {
        if(_isTakingDamage) { return; }
        if (_isUsingLeftSide)
        {
            if(_currentArmRotationAngleFromRight < 90.0f)
            {
                // At   0 degrees it is at frame 0
                // At  90 degrees it is at frame 10
                // ((10 * x) / 90) * (1 / 30) = x / 270
                _gunArmTrack.TrackTime = _currentArmRotationAngleFromRight / 270.0f;
            }
            else if(_currentArmRotationAngleFromRight <= 315.0f)
            {
                // At  90 degrees it is at frame 10
                // At 315 degrees it is at frame 60
                // ((x - 90) / (315 - 90)) * (60 - 10) + 10 * (1 / 30) = (x - 45) / 135
                _gunArmTrack.TrackTime = (_currentArmRotationAngleFromRight - 45.0f) / 135.0f;
            }
            else 
            {
                Debug.LogWarning("Shouldn't be using this side arm right now (Left)");
                _gunArmTrack.TrackTime = 2.0f;
            }
        }
        else
        {
            if (_currentArmRotationAngleFromRight <= 180.0f && _currentArmRotationAngleFromRight > 90.0f)
            {
                // At 180 degrees it is at frame 0
                // At  90 degrees it is at frame 10
                // (1 - (x - 90) / 90) * 10 * (1 / 30) = (180 - x) / 270
                _gunArmTrack.TrackTime = (180 - _currentArmRotationAngleFromRight) / 270.0f;
            }
            else if (_currentArmRotationAngleFromRight <= 90.0f)
            {
                // At  90 degrees it is at frame 10
                // At   0 degrees it is at frame 30
                // (((1 - (x / 90)) * 20) + 10) * (1 / 30) = (135 - x) / 135
                _gunArmTrack.TrackTime = (135.0f - _currentArmRotationAngleFromRight) / 135.0f;
            }
            else if(_currentArmRotationAngleFromRight <= 360.0f && _currentArmRotationAngleFromRight >= 225.0f)
            {
                // At 360 degrees it is at frame 30
                // At 225 degrees it is at frame 60
                // (((1 - ((x - 225) / (360 - 225))) * 30) + 30) * (1 / 30) = (495 - x) / 135 
                _gunArmTrack.TrackTime = (495.0f - _currentArmRotationAngleFromRight) / 135.0f; // Just the above equation + (360 / 135)
            }
            else
            {
                Debug.LogWarning("Shouldn't be using this side arm right now (Right)");
                _gunArmTrack.TrackTime = 2.0f;
            }
        }
    }

    public void StartDamageAnimation()
    {
        _isTakingDamage = true;
        SpineAnimationState.SetAnimation(1, DamageAnimation, false);
    }

    public void EndDamageAnimation()
    {
        _isTakingDamage = false;
        SpineAnimationState.SetEmptyAnimation(1, 0.0f);

        switch (_currentDirection) // Damage animation is flat so need to change based on direction
        {
            case FacingDirection.Front:
                SpineSkeleton.ScaleX = 1.0f;
                SetArmSide(true);
                SpineAnimationState.SetAnimation(0, FrontDirection, _skeletonAnimation);
                break;
            case FacingDirection.Back:
                SpineSkeleton.ScaleX = 1.0f;
                SetArmSide(false);
                SpineAnimationState.SetAnimation(0, BackDirection, _skeletonAnimation);
                break;
            case FacingDirection.Left:
                SpineSkeleton.ScaleX = 1.0f;
                SetArmSide(true);
                SpineAnimationState.SetAnimation(0, SideDirection, _skeletonAnimation);
                break;
            case FacingDirection.Right: // Right side animations are funky as the whole SpineSkeleton's scale is reversed
                SpineSkeleton.ScaleX = -1.0f;
                SetArmSide(true);
                SpineAnimationState.SetAnimation(0, SideDirection, _skeletonAnimation);
                break;
        }

        UpdateAimDirection();
    }
}
