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
    private PathfindingManager _pathfindingManager;
    private Rigidbody2D _rb;
    private GameObject _player;
    private GameObject _dashRechargeZone;

    private GameObject _nearestTarget;
    private GameObject _playerTarget;
    private GameObject _currentTarget;

    private CompanionStates _state;

    private float _leapTimer;
    private bool _isLeapFinished;
    private float _leapMoveTimer;
    private float _leapEndTimer;

    private bool _isReadyToLeap;

    private Vector2 _leapStart;
    private Vector2 _leapEnd;

    private bool _isDashRefreshSpawned;

    private Node _lastPlayerNode;
    private Node _lastTargetNode;
    private Node _lastNode;
    private Vector2 _lastPathDirection;

    public void InitialiseComponent(ref CompanionFriendData dataObj, ref CompanionDetection detectionScript, ref CompanionAnimations animationScript, ref PathfindingManager pathfindingManager, ref Rigidbody2D rb, ref GameObject player, ref GameObject dashRechargeZone)
    {
        _dataObj = dataObj;
        _detectionScript = detectionScript;
        _animationScript = animationScript;
        _pathfindingManager = pathfindingManager;
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

                Vector2 travelDirection = CompanionPathfindingToPlayer();

                _rb.MovePosition(new Vector2(_rb.position.x, _rb.position.y) + travelDirection * travelDistance);

                break;
            case CompanionStates.ATTACKING:

                if (!_isReadyToLeap)
                {
                    Node targetNode = _pathfindingManager.NodeFromWorldPosition(_currentTarget.transform.position);
                    Node currentNode = _pathfindingManager.NodeFromWorldPosition(transform.position);

                    Vector2 pathfindingDirection = _lastPathDirection;

                    if (_lastTargetNode != targetNode || _lastNode != currentNode)
                    {
                        pathfindingDirection = _pathfindingManager.GetPathDirection(transform.position, _currentTarget.transform.position);

                        _lastTargetNode = targetNode;
                        _lastNode = currentNode;
                        _lastPathDirection = pathfindingDirection;
                    }

                    _rb.MovePosition(new Vector2(transform.position.x, transform.position.y) + pathfindingDirection * _dataObj.moveSpeed * Time.fixedDeltaTime);

                    if (WithinLeapRange(_dataObj.leapDistance, _currentTarget))
                    {
                        Vector2 targetDirection = _currentTarget.transform.position - transform.position;
                        Vector2 leapDirection = targetDirection.normalized;
                        if (targetDirection == Vector2.zero)
                        {
                            leapDirection = (_leapEnd - _leapStart).normalized;
                        }
                        _leapStart = transform.position;
                        _leapEnd = _leapStart + leapDirection * _dataObj.leapDistance;

                        RaycastHit2D wallCheck = Physics2D.Raycast(_leapStart + leapDirection * 0.1f, leapDirection, _dataObj.leapDistance, _dataObj.environmentLayer);
                        if (wallCheck)
                        {
                            _leapEnd = wallCheck.point;
                        }

                        _leapEnd = _pathfindingManager.GetNearestNodeInDirection(_leapEnd, new Vector3(-leapDirection.x, -leapDirection.y, 0.0f)).worldPosition; // Find the nearest unblocked node along the path


                        _leapTimer = Time.time;

                        _isReadyToLeap = true;

                        _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.LEAP_MOVING);
                    }
                    return;
                }

                if (Time.time - _leapTimer < _dataObj.leapChargeTime)
                {
                    _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.LEAP_CHARGE);
                    _leapMoveTimer = Time.time;
                    return;
                }

                float travelPosition = (Time.time - _leapMoveTimer) / _dataObj.leapTravelTime;

                _rb.MovePosition(Vector2.Lerp(_leapStart, _leapEnd, travelPosition));

                if (travelPosition >= 1)
                {
                    _currentTarget = null;
                    _isLeapFinished = true;
                }

                break;

        }
    }
    private bool WithinLeapRange(float leapTravelDistance, GameObject target)
    {
        // Leaping
        Vector3 targetDirection = target.transform.position - transform.position;
        Vector3 leapDirection = targetDirection.normalized;

        float leapDistance = leapTravelDistance * _dataObj.leapTargetTravelPercentage;
        leapDistance *= leapDistance;

        if ((target.transform.position - transform.position).sqrMagnitude > leapDistance)
        {
            return false;
        }

        RaycastHit2D wallCheck = Physics2D.Raycast(transform.position + leapDirection * 0.1f, leapDirection, leapTravelDistance, _dataObj.environmentLayer); // Update layer mask variable
        if (wallCheck)
        {
            float wallDistance = (wallCheck.point - new Vector2(transform.position.x, transform.position.y)).sqrMagnitude;

            if (wallDistance < leapDistance)
            {
                return false;
            }
        }
        return true;
    }

    private Vector2 CompanionPathfindingToPlayer()
    {
        Node playerNode = _pathfindingManager.NodeFromWorldPosition(_player.transform.position);
        Node currentNode = _pathfindingManager.NodeFromWorldPosition(transform.position);
        if (_lastPlayerNode == playerNode && _lastNode == currentNode)
        {
            return _lastPathDirection;
        }

        _lastPlayerNode = playerNode;
        _lastNode = currentNode;
        Vector2 direction = _pathfindingManager.GetPathDirection(_rb.position, _player.transform.position);
        _lastPathDirection = direction;
        return direction;
    }

    public void SetPlayerTarget(GameObject target)
    {
        _playerTarget = target;
    }

    public void RemovePlayerTarget(GameObject target)
    {
        if(_playerTarget == target)
        {
            _playerTarget = null;
        }
    }

    public void IdleAction()
    {
        _nearestTarget = _detectionScript.GetTarget();

        if(_nearestTarget != null)
        {
            _currentTarget = _nearestTarget;
        }
        if(_playerTarget != null) 
        {
            _currentTarget = _playerTarget;
        }

        if (_currentTarget != null) // Found target to attack
        {
            _state = CompanionStates.ATTACKING;
            _leapTimer = Time.time;
            _isReadyToLeap = false;
            _isLeapFinished = false;
            _isDashRefreshSpawned = false;

            // To be updated later
            if (_currentTarget.transform.position.x - transform.position.x < 0)
            {
                _animationScript.ChangeAnimationDirection(CompanionAnimations.FacingDirection.LEFT);
            }
            else if (_currentTarget.transform.position.x - transform.position.x > 0)
            {
                _animationScript.ChangeAnimationDirection(CompanionAnimations.FacingDirection.RIGHT);
            }

            return;
        }

        _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.IDLE);
    }

    public void Leap()
    {
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
}