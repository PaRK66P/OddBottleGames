using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionAnimations : MonoBehaviour
{
    public enum AnimationState
    {
        IDLE,

        LEAP_CHARGE,
        LEAP_MOVING,
        LEAP_END,

        SPIT_CHARGE,
        SPIT_ATTACK,
        SPIT_END,

        LICK_CHARGE,
        LICK_ATTACK,
        LICK_END,

        SCREAM_CHARGE,
        SCREAM_ATTACK,
        SCREAM_END
    }

    public enum FacingDirection
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    CompanionBossData _bossData;
    CompanionFriendData _friendData;
    SpriteRenderer _spriteRenderer;

    private AnimationState _animState;
    private FacingDirection _facingDirection;

    private bool _isFacingRightLast; // Between left and right

    public void InitialiseComponent(ref CompanionBossData bossData, ref CompanionFriendData friendData, ref SpriteRenderer spriteRenderer)
    {
        _bossData = bossData;
        _friendData = friendData;
        _spriteRenderer = spriteRenderer;
    }

    public void ChangeAnimationState(AnimationState animation)
    {
        if(_animState == animation) return;

        _animState = animation;
        if (_animState == AnimationState.LEAP_CHARGE)
        {
            _spriteRenderer.transform.localScale = new Vector3(Mathf.Abs(_spriteRenderer.transform.localScale.x), _spriteRenderer.transform.localScale.y, _spriteRenderer.transform.localScale.z);
        }
        else
        {
            _spriteRenderer.transform.localScale = new Vector3(Mathf.Abs(_spriteRenderer.transform.localScale.x) * (_isFacingRightLast ? 1 : -1), _spriteRenderer.transform.localScale.y, _spriteRenderer.transform.localScale.z);
        }
        _spriteRenderer.sprite = _bossData.GetSprite(_animState, _isFacingRightLast);
    }

    public void ChangeAnimationDirection(FacingDirection direction)
    {
        if (direction == FacingDirection.LEFT)
        {
            _isFacingRightLast = false;
        }
        else if (direction == FacingDirection.RIGHT)
        {
            _isFacingRightLast = true;
        }

        _facingDirection = direction;
    }
}
