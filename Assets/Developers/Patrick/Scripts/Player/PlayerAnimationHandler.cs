using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

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

    [Header("Gun Arms")]
    [SpineAnimation]
    public string leftSide;
    [SpineAnimation]
    public string rightSide;

    private SkeletonAnimation skeletonAnimation;

    public Spine.AnimationState spineAnimationState;
    public Spine.Skeleton skeleton;

    private FacingDirection currentDirection = FacingDirection.FRONT;

    private bool _isMoving = false;
    private bool _isDashing = false;

    private Spine.TrackEntry _gunArmTrack;
    private float _currentArmRotationAngleFromRight;
    private bool _isUsingLeftSide = true;

    public float frame;

    // Start is called before the first frame update
    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        spineAnimationState = skeletonAnimation.AnimationState;
        skeleton = skeletonAnimation.Skeleton;

        _gunArmTrack = spineAnimationState.SetAnimation(2, leftSide, true);
        _gunArmTrack.TimeScale = 0.0f;

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
                skeleton.ScaleX = 1.0f;
                SetArmSide(true);
                spineAnimationState.SetAnimation(0, frontDirection, skeletonAnimation);
                break;
            case FacingDirection.BACK:
                skeleton.ScaleX = 1.0f;
                SetArmSide(false);
                spineAnimationState.SetAnimation(0, backDirection, skeletonAnimation);
                break;
            case FacingDirection.LEFT:
                skeleton.ScaleX = 1.0f;
                SetArmSide(true);
                spineAnimationState.SetAnimation(0, sideDirection, skeletonAnimation);
                break;
            case FacingDirection.RIGHT: // Right side animations are funky as the whole skeleton's scale is reversed
                skeleton.ScaleX = -1.0f;
                SetArmSide(true);
                spineAnimationState.SetAnimation(0, sideDirection, skeletonAnimation);
                break;
        }

        UpdateAimDirection();

        currentDirection = facingDirection;
    }

    public void StartDashAnimation()
    {
        spineAnimationState.SetAnimation(1, dashAnimation, false);
    }

    public void EndDashAnimation()
    {
        if (_isMoving)
        {
            spineAnimationState.SetAnimation(1, runAnimation, true);
        }
        else
        {
            spineAnimationState.SetEmptyAnimation(1, 0.0f);
        }
        
    }

    public void UpdateMovementAnimation(bool isMoving)
    {
        if(_isMoving == isMoving) { return; }

        _isMoving = isMoving;

        if (_isDashing) {  return; }

        if (_isMoving)
        {
            spineAnimationState.SetAnimation(1, runAnimation, true);
        }
        else
        {
            spineAnimationState.SetEmptyAnimation(1, 0.0f);
        }
    }

    private void SetArmSide(bool isLeftSide)
    {
        if (isLeftSide == _isUsingLeftSide) { return; }
        _isUsingLeftSide = isLeftSide;

        if (_isUsingLeftSide)
        {
            _gunArmTrack = spineAnimationState.SetAnimation(2, leftSide, true);
        }
        else
        {
            _gunArmTrack = spineAnimationState.SetAnimation(2, rightSide, true);
        }

        _gunArmTrack.TimeScale = 0.0f;

    }


    public void SetAimDirection(Vector2 direction)
    {
        if(currentDirection == FacingDirection.RIGHT) { direction.x *= -1.0f; }

        float AngleFromRight = Vector3.SignedAngle(Vector3.right, direction, new Vector3(0.0f, 0.0f, 1.0f));

        if (Mathf.Sign(AngleFromRight) == -1.0f)
        {
            AngleFromRight += 360.0f;
        }

        _currentArmRotationAngleFromRight = AngleFromRight;
        UpdateAimDirection();
    }

    private void UpdateAimDirection()
    {
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
                Debug.LogWarning("Shouldn't be using this side arm right now (LEFT)");
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
                Debug.LogWarning("Shouldn't be using this side arm right now (RIGHT)");
                _gunArmTrack.TrackTime = 2.0f;
            }
        }
    }
}
