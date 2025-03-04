using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionFriend : MonoBehaviour
{
    enum CompanionStates
    {
        IDLE = 0,
        ATTACKING = 1,
        DELAY = 2,
        STATIC = 3
    }

    private CompanionFriendData _dataObj;
    private CompanionDetection _detectionScript;
    private CompanionAnimations _animationScript;
    private Rigidbody2D _rb;
    private GameObject _player;
    private GameObject _dashRechargeZone;

    private GameObject _target;

    private CompanionStates _state;

    private float _leapTimer;
    private bool _isLeapMoving;
    private bool _isLeapFinished;
    private float _leapMoveTimer;
    private float _leapEndTimer;

    private Vector2 _leapStart;
    private Vector2 _leapEnd;

    private bool _isLeapCalculated;
    private bool _isDashRefreshSpawned;

    public void InitialiseComponent(ref CompanionFriendData dataObj, ref CompanionDetection detectionScript, ref CompanionAnimations animationScript, ref Rigidbody2D rb, ref GameObject player, ref GameObject dashRechargeZone)
    {
        _dataObj = dataObj;
        _detectionScript = detectionScript;
        _animationScript = animationScript;
        _rb = rb;
        _player = player;
        _dashRechargeZone = dashRechargeZone;
    }

    public void CompanionUpdate()
    {
        switch (_state)
        {
            case CompanionStates.IDLE:
                IdleAction();
                break;
            case CompanionStates.ATTACKING:
                Leap();
                break;
        }
    }

    public void CompanionFixedUpdate()
    {
        switch (_state)
        {
            case CompanionStates.IDLE:
                float travelDistance = _dataObj.idleSpeed * Time.fixedDeltaTime;
                Vector2 playerDistance = new Vector2(_player.transform.position.x, _player.transform.position.y) - _rb.position;

                if(playerDistance.sqrMagnitude <= _dataObj.idleDistance * _dataObj.idleDistance)
                {
                    break;
                }

                playerDistance = playerDistance - (playerDistance.normalized * _dataObj.idleDistance);

                if (playerDistance.sqrMagnitude < travelDistance * travelDistance)
                {
                    travelDistance = playerDistance.magnitude;
                }

                _rb.MovePosition(new Vector2(_rb.position.x, _rb.position.y) + playerDistance.normalized * travelDistance);

                break;
            case CompanionStates.ATTACKING:

                if (!_isLeapMoving)
                {
                    return;
                }

                float travelPosition = (Time.time - _leapMoveTimer) / _dataObj.leapTravelTime;

                _rb.MovePosition(Vector2.Lerp(_leapStart, _leapEnd, travelPosition));

                if (travelPosition >= 1)
                {
                    _isLeapFinished = true;
                }

                break;

        }
    }

    public void IdleAction()
    {
        _target = _detectionScript.GetTarget();

        if (_target != null) // Found target to attack
        {
            _state = CompanionStates.ATTACKING;
            _leapTimer = Time.time;
            _isLeapCalculated = false;
            _isLeapFinished = false;
            _isDashRefreshSpawned = false;
            _isLeapMoving = false;

            // To be updated later
            if (_target.transform.position.x - transform.position.x < 0)
            {
                _animationScript.ChangeAnimationDirection(CompanionAnimations.FacingDirection.LEFT);
            }
            else if (_target.transform.position.x - transform.position.x > 0)
            {
                _animationScript.ChangeAnimationDirection(CompanionAnimations.FacingDirection.RIGHT);
            }

            return;
        }

        _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.IDLE);
    }

    public void Leap()
    {
        if(Time.time - _leapTimer < _dataObj.leapChargeTime)
        {
            _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.LEAP_CHARGE);
            return;
        }

        if (!_isLeapCalculated)
        {
            Vector2 targetDirection = _target.transform.position - transform.position;
            Vector2 leapDirection = targetDirection.normalized;
            if (targetDirection == Vector2.zero)
            {
                leapDirection = (_leapEnd - _leapStart).normalized;
            }
            _leapStart = transform.position;
            _leapEnd = _leapStart + leapDirection * _dataObj.leapDistance;
            if (targetDirection.sqrMagnitude >= _dataObj.leapDistance * _dataObj.leapDistance)
            {
                _leapEnd = _target.transform.position;
            }
            RaycastHit2D wallCheck = Physics2D.Raycast(_leapStart + leapDirection * 0.1f, leapDirection, _dataObj.leapDistance, _dataObj.environmentLayer);
            if (wallCheck)
            {
                _leapEnd = wallCheck.point;
            }

            _isLeapMoving = true;
            _leapMoveTimer = Time.time;

            _isLeapCalculated = true;

            _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.LEAP_MOVING);
        }

        if (!_isLeapFinished)
        {
            return;
        }

        _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.LEAP_END);

        if (!_isDashRefreshSpawned)
        {
            //SpawnDashRefresh();
            _leapEndTimer = Time.time;
            _isDashRefreshSpawned = true;
        }

        if(Time.time - _leapEndTimer < _dataObj.leapEndTime)
        {
            return;
        }

        //DespawnDashRefresh();
        _state = CompanionStates.IDLE;

    }

    private void SpawnDashRefresh()
    {
        _dashRechargeZone.SetActive(true);
    }

    private void DespawnDashRefresh()
    {
        _dashRechargeZone.SetActive(false);
    }
}