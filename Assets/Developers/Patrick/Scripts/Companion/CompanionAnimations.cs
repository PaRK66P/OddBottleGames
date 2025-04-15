using UnityEngine;

public class CompanionAnimations : MonoBehaviour
{
    public enum AnimationState
    {
        Idle,

        LeapCharge,
        LeapMove,
        LeapEnd,

        SpitCharge,
        SpitAttack,
        SpitEnd,

        LickCharge,
        LickAttack,
        LickEnd,

        ScreamCharge,
        ScreamAttack,
        ScreamEnd
    }

    public enum FacingDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    // Data
    CompanionBossData _bossData;

    // Components
    SpriteRenderer _spriteRenderer;

    // Values
    private AnimationState _animState;
    private bool _isFacingRightLast; // Between left and right

    public void InitialiseComponent(ref CompanionBossData bossData, ref SpriteRenderer spriteRenderer)
    {
        _bossData = bossData;
        _spriteRenderer = spriteRenderer;
    }

    // To be changed
    // Works with specific animations (not modular)
    public void ChangeAnimationState(AnimationState animation)
    {
        if(_animState == animation) return;

        _animState = animation;
        if (_animState == AnimationState.LeapCharge) // Has different sprites for direction so don't change transform
        {
            _spriteRenderer.transform.localScale = new Vector3(Mathf.Abs(_spriteRenderer.transform.localScale.x), _spriteRenderer.transform.localScale.y, _spriteRenderer.transform.localScale.z);
        }
        else // Update facing direction
        {
            _spriteRenderer.transform.localScale = new Vector3(Mathf.Abs(_spriteRenderer.transform.localScale.x) * (_isFacingRightLast ? 1 : -1), _spriteRenderer.transform.localScale.y, _spriteRenderer.transform.localScale.z);
        }
        _spriteRenderer.sprite = _bossData.GetSprite(_animState, _isFacingRightLast); // Change animation based on boss data
    }

    public void ChangeAnimationDirection(FacingDirection direction)
    {
        if (direction == FacingDirection.Left)
        {
            _isFacingRightLast = false;
        }
        else if (direction == FacingDirection.Right)
        {
            _isFacingRightLast = true;
        }
    }
}
