using UnityEngine;

public class CompanionFriend : MonoBehaviour
{
    enum CompanionStates
    {
        Idle,
        Attacking,
        Delay,
        Static
    }

    // Data
    private CompanionFriendData _dataObj;

    // Objects
    private GameObject _player;
    private GameObject _nearestTarget;
    private GameObject _playerTarget;
    private GameObject _currentTarget;
    private PathfindingManager _pathfindingManager;
    private SoundManager _soundManager;

    // Components
    private Rigidbody2D _rb;
    private CompanionDetection _detectionScript;
    private CompanionAnimationHandler _animationScript;

    // Values
    private CompanionStates _state;
    private Node _lastPlayerNode;
    private Node _lastTargetNode;
    private Node _lastNode;
    private Vector2 _lastPathDirection;

    // Leap attack
    private float _leapTimer;
    private bool _isLeapFinished;
    private float _leapMoveTimer;
    private float _leapEndTimer;

    private bool _isReadyToLeap;
    private bool _isLeapChargeStarted;

    private Vector2 _leapStart;
    private Vector2 _leapEnd;

    public void InitialiseComponent(ref CompanionFriendData dataObj, ref CompanionDetection detectionScript, ref CompanionAnimationHandler animationScript, ref PathfindingManager pathfindingManager, ref SoundManager soundManager, ref Rigidbody2D rb, ref GameObject player)
    {
        _dataObj = dataObj;
        _detectionScript = detectionScript;
        _animationScript = animationScript;
        _pathfindingManager = pathfindingManager;
        _soundManager = soundManager;
        _rb = rb;
        _player = player;
    }

    #region Update
    public void CompanionUpdate()
    {
        // Update based on state
        Debug.Log("Companion State: " + _state.ToString());
        switch (_state)
        {
            case CompanionStates.Idle:
                IdleAction();
                break;
            case CompanionStates.Attacking:
                Leap();
                break;
        }
    }

    public void CompanionFixedUpdate()
    {
        _soundManager.SetWalkingAmb(false); // Assume false until not
        // Fixed update based on state
        switch (_state)
        {
            case CompanionStates.Idle: // Move towards the player
                UpdateDirection();

                float travelDistance = _dataObj.IdleSpeed * Time.fixedDeltaTime;
                Vector2 playerDistance = new Vector2(_player.transform.position.x, _player.transform.position.y) - _rb.position;

                if(playerDistance.sqrMagnitude <= _dataObj.IdleDistance * _dataObj.IdleDistance) // If the player is close enough, no need to move
                {
                    _animationScript.SetToIdleAnimation();
                    _animationScript.ChangeAnimationTrackSpeed(0.5f);
                    break;
                }

                Vector2 travelDirection = CompanionPathfindingToPlayer();

                _soundManager.SetWalkingAmb(true);
                _animationScript.SetToRunAnimation();
                _animationScript.ResetAnimationTrackSpeed();
                _rb.MovePosition(new Vector2(_rb.position.x, _rb.position.y) + travelDirection * travelDistance);

                break;
            case CompanionStates.Attacking: // Attack is the leap attack
                // Not ready when not in range to leap yet
                if (!_isReadyToLeap)
                {
                    Node targetNode = _pathfindingManager.NodeFromWorldPosition(_currentTarget.transform.position);
                    Node currentNode = _pathfindingManager.NodeFromWorldPosition(transform.position);

                    Vector2 pathfindingDirection = _lastPathDirection;

                    if (_lastTargetNode != targetNode || _lastNode != currentNode) // Only need to pathfind if positions change
                    {
                        pathfindingDirection = _pathfindingManager.GetPathDirection(transform.position, _currentTarget.transform.position);

                        _lastTargetNode = targetNode;
                        _lastNode = currentNode;
                        _lastPathDirection = pathfindingDirection;
                    }

                    _soundManager.SetWalkingAmb(true);
                    UpdateDirection();
                    _animationScript.SetToRunAnimation();
                    // Move closer to the target
                    _rb.MovePosition(new Vector2(transform.position.x, transform.position.y) + pathfindingDirection * _dataObj.MoveSpeed * Time.fixedDeltaTime);

                    // Check if ready
                    if (WithinLeapRange(_dataObj.LeapDistance, _currentTarget))
                    {
                        Vector2 targetDirection = _currentTarget.transform.position - transform.position;
                        Vector2 leapDirection = targetDirection.normalized;
                        if (targetDirection == Vector2.zero)
                        {
                            leapDirection = (_leapEnd - _leapStart).normalized;
                        }
                        _leapStart = transform.position;
                        _leapEnd = _leapStart + leapDirection * _dataObj.LeapDistance;

                        // Ensures the end is not on the opposite side of a wall
                        RaycastHit2D wallCheck = Physics2D.Raycast(_leapStart + leapDirection * 0.1f, leapDirection, _dataObj.LeapDistance, _dataObj.EnvironmentLayer);
                        if (wallCheck)
                        {
                            _leapEnd = wallCheck.point;
                        }

                        _leapEnd = _pathfindingManager.GetNearestNodeInDirection(_leapEnd, new Vector3(-leapDirection.x, -leapDirection.y, 0.0f)).worldPosition; // Find the nearest unblocked node along the path

                        _leapTimer = Time.time;

                        _isReadyToLeap = true;
                        _isLeapChargeStarted = false;
                    }
                    return;
                }

                // Charge up
                if (Time.time - _leapTimer < _dataObj.LeapChargeTime)
                {
                    if (!_isLeapChargeStarted)
                    {
                        _isLeapChargeStarted = true;
                        _soundManager.PlayAmbDashReady(2);
                    }

                    _animationScript.StartLeap();
                    _leapMoveTimer = Time.time;
                    return;
                }

                // Movement

                float travelPosition = (Time.time - _leapMoveTimer) / _dataObj.LeapTravelTime;

                _rb.MovePosition(Vector2.Lerp(_leapStart, _leapEnd, travelPosition));

                _animationScript.PlayLeapMovement();

                if (travelPosition >= 1) // Once the leap is finished
                {
                    _soundManager.PlayAmbDashAttack(2);

                    _currentTarget = null;
                    _isLeapFinished = true;
                }

                break;

        }
    }
    #endregion

    #region Helper Methods
    // Returns if the target is within the leap distance of the companion (takes environment into account)
    private bool WithinLeapRange(float leapTravelDistance, GameObject target)
    {
        // Leaping
        Vector3 targetDirection = target.transform.position - transform.position;
        Vector3 leapDirection = targetDirection.normalized;

        float leapDistance = leapTravelDistance * _dataObj.LeapTargetTravelPercentage;
        leapDistance *= leapDistance; // Cheaper to compare square magnitudes than square roots of magnitudes

        // Check if the target is within the distance
        if ((target.transform.position - transform.position).sqrMagnitude > leapDistance)
        {
            return false;
        }

        // Check if the target is on the other side of a wall
        RaycastHit2D wallCheck = Physics2D.Raycast(transform.position + leapDirection * 0.1f, leapDirection, leapTravelDistance, _dataObj.EnvironmentLayer); // Update layer mask variable
        if (wallCheck)
        {
            float wallDistance = (wallCheck.point - new Vector2(transform.position.x, transform.position.y)).sqrMagnitude;

            if (wallDistance < leapDistance)
            {
                return false;
            }
        }

        // Target is within the distance and not behind a wall
        return true;
    }

    // Returns the direction of the path towards the player
    private Vector2 CompanionPathfindingToPlayer()
    {
        Node playerNode = _pathfindingManager.NodeFromWorldPosition(_player.transform.position);
        Node currentNode = _pathfindingManager.NodeFromWorldPosition(transform.position);
        if (_lastPlayerNode == playerNode && _lastNode == currentNode) // Only need to pathfind if positions change
        {
            return _lastPathDirection;
        }

        _lastPlayerNode = playerNode;
        _lastNode = currentNode;
        Vector2 direction = _pathfindingManager.GetPathDirection(_rb.position, _player.transform.position);
        _lastPathDirection = direction;
        return direction;
    }

    // Sets the enemy the player is targetting
    public void SetPlayerTarget(GameObject target)
    {
        _playerTarget = target;
    }

    // Removes the enemy the player's targetting
    public void RemovePlayerTarget(GameObject target)
    {
        if(_playerTarget == target)
        {
            _playerTarget = null;
        }
    }
    #endregion

    #region Actions
    // Will move towards the player
    public void IdleAction()
    {
        // Checks if there is a target to go after
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
            _state = CompanionStates.Attacking;
            _leapTimer = Time.time;
            _isReadyToLeap = false;
            _isLeapFinished = false;

            _animationScript.ResetAnimationTrackSpeed();

            return;
        }
    }

    // Companion will jump towards the target, damaging on collision
    /*
     * Because the leap attack is mainly movement
     * Most of the attack is handled within the fixed update loop
     */
    public void Leap()
    {
        // Wait for leap to finish in fixed update
        if (!_isLeapFinished)
        {
            return;
        }

        // Delay for the end of the attack
        if(Time.time - _leapEndTimer < _dataObj.LeapEndTime)
        {
            return;
        }

        // End attack
        _state = CompanionStates.Idle;
    }
    #endregion

    public GameObject GetTarget()
    {
        return _currentTarget;
    }
    private void UpdateDirection()
    {
        Vector3 direction = _player.transform.position - transform.position;

        float AngleFromRight = Vector3.SignedAngle(Vector3.right, direction, new Vector3(0.0f, 0.0f, 1.0f));
        if (AngleFromRight > -45.0f && AngleFromRight < 45.0f) { _animationScript.UpdateFacingDirection(CompanionAnimationHandler.FacingDirection.Right); }
        else if (AngleFromRight >= 45.0f && AngleFromRight <= 135.0f) { _animationScript.UpdateFacingDirection(CompanionAnimationHandler.FacingDirection.Back); }
        else if (AngleFromRight > 135.0f || AngleFromRight < -135.0f) { _animationScript.UpdateFacingDirection(CompanionAnimationHandler.FacingDirection.Left); }
        else if (AngleFromRight >= -135.0f || AngleFromRight <= -45.0f) { _animationScript.UpdateFacingDirection(CompanionAnimationHandler.FacingDirection.Front); }
    }
}