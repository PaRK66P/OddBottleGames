using UnityEngine;

public class CompanionBoss : MonoBehaviour
{
    enum AttackState
    {
        Leap = 0,
        Spit = 1,
        Lick = 2,
        Scream = 3,
        Feral = 4,
        Delay = 5
    }

    // Data
    private CompanionBossData _dataObj;

    // Objects
    private GameObject _player;
    private PathfindingManager _pathfindingScript;
    private ObjectPoolManager _poolManager;
    private SoundManager _soundManager;

    // Components
    private Rigidbody2D _rb;
    private CompanionAnimationHandler _animationScript;

    // Attacks
    private AttackState _currentState;
    private float _attackEndDelay;
    private bool _isLastAttackLeap;
    private int _leapAmount;
    private bool _doLickAttack;

    // Leap Attack
    private float _leapStartTimer;
    private float _leapMoveTimer;
    private Vector2 _leapStart;
    private Vector2 _leapEnd;
    private bool _isLeapMoving;
    private bool _isLeapFinished;
    private bool _isReadyToLeap;
    private float _readyStartTime;
    private Node _lastPlayerNode;
    private Node _lastNode;
    private Vector3 _lastLeapMoveDirection;
    private float _leapTravelDistance;
    private float _leapChargeTime;
    private float _leapTravelTime;
    private float _leapEndTime;
    private bool _isLeapChargeStarted;

    // Feral Attack
    private int _feralLeapAmount;
    private int _feralLeapCurrentAmount;

    // Spit Attack
    private float _spitStartTimer;
    private float _spitTravelDistance;
    private float _spitSize;

    // Lick Attack
    private float _lickStartTimer;
    private int _lickWaveCurrentCount;
    private int _lickWaveCount;
    private float _lickLastWaveStartTime;
    private float _lickWaveGap;
    private float _lickProjectileSpeed;
    private float _lickProjectiles;
    private float _lickLastWaveProjectiles;
    private bool _isLickStarted;

    // Scream Attack
    private float _screamStartTimer;
    private int _screamWaveCurrentCount;
    private int _screamWaveCount;
    private float _screamLastWaveStartTime;
    private float _screamWaveGap;
    private float _screamProjectileSpeed;
    private Vector2 _screamStartDirection;

    // Heat up
    private int _heatUpStage;

    #region Set Up
    // Sets up the component
    public void InitialiseComponent(ref CompanionBossData bossData, 
        ref Rigidbody2D rigidbodyComp, ref CompanionAnimationHandler animationScript, 
        ref PathfindingManager pathfindingScript, ref GameObject playerObject, 
        ref ObjectPoolManager poolManager, ref SoundManager soundManager)
    {
        _dataObj = bossData;
        _rb = rigidbodyComp;
        _animationScript = animationScript;
        _pathfindingScript = pathfindingScript;
        _player = playerObject;
        _poolManager = poolManager;
        _soundManager = soundManager;

        _heatUpStage = 1;
    }

    // Sets up the companion to boss mode
    public void SetupEnemy()
    {
        _currentState = AttackState.Delay;

        _isLastAttackLeap = false;
        _leapAmount = 0;
        _doLickAttack = false; // Start with Scream

        _leapStartTimer = 0;
        _leapMoveTimer = 0;
        _isLeapMoving = false;
        _isLeapFinished = false;

        _feralLeapCurrentAmount = 0;
    }
    #endregion

    #region Updates
    // Updates based on the current state
    public void CompanionUpdate()
    {
        switch (_currentState)
        {
            case AttackState.Leap:
                LeapAttack();
                break;
            case AttackState.Spit:
                SpitAttack();
                break;
            case AttackState.Lick:
                LickAttack();
                break;
            case AttackState.Scream:
                ScreamAttack();
                break;
            case AttackState.Feral:
                FeralLeapAttack();
                break;
            case AttackState.Delay:
                Delay();
                break;
            default:
                Debug.LogError("Companion _currentState is unknown");
                break;
        }
    }

    // Only needed for Leap movement
    public void CompanionFixedUpdate()
    {
        if (_currentState != AttackState.Leap && _currentState != AttackState.Feral) // Skip if not leaping
        {
            return;
        }

        // Not ready when not in range to leap yet
        if (!_isReadyToLeap)
        {
            _soundManager.SetWalkingAmb(true);

            // Find direction
            Node playerNode = _pathfindingScript.NodeFromWorldPosition(_player.transform.position);
            Node currentNode = _pathfindingScript.NodeFromWorldPosition(transform.position);
            if (_lastPlayerNode != playerNode || _lastNode != currentNode) // Only need to pathfind if positions change
            {
                _lastLeapMoveDirection = _pathfindingScript.GetPathDirection(transform.position, _player.transform.position);
                _lastPlayerNode = playerNode;
                _lastNode = currentNode;
            }

            // Move closer to the player
            _rb.MovePosition(transform.position + _lastLeapMoveDirection * _dataObj.MoveSpeed * Time.fixedDeltaTime * Mathf.Max((_dataObj.MoveSpeedMultiplier * (Time.time - _readyStartTime)), 1.0f));
            _animationScript.ChangeAnimationTrackSpeed(Mathf.Max((_dataObj.MoveSpeedMultiplier * (Time.time - _readyStartTime)), 1.0f));

            // Check if ready
            if (WithinLeapRange(_leapTravelDistance))
            {
                _leapStartTimer = Time.time;
                _leapStart = transform.position; // Logically will always be in a not blocked node so this SHOULD be safe
                Vector2 playerDirection = _player.transform.position - transform.position;
                Vector2 leapDirection = playerDirection.normalized;

                _leapEnd = _leapStart + leapDirection * (_leapTravelDistance);
                
                // Ensures the end is not on the opposite side of a wall
                RaycastHit2D wallCheck = Physics2D.Raycast(_leapStart + leapDirection * 0.1f, leapDirection, _leapTravelDistance, _dataObj.EnvironmentMask);
                if (wallCheck)
                {
                    _leapEnd = wallCheck.point;
                }

                _leapEnd = _pathfindingScript.GetNearestNodeInDirection(_leapEnd, new Vector3(-leapDirection.x, -leapDirection.y, 0.0f)).worldPosition; // Find the nearest unblocked node along the path

                _leapStartTimer = Time.time;
                _isReadyToLeap = true;
                _isLeapChargeStarted = false;

                _animationScript.ResetAnimationTrackSpeed();
            }

            UpdateDirection();
            return;
        }
        _soundManager.SetWalkingAmb(false);

        // Charge up
        if (Time.time - _leapStartTimer <= _leapChargeTime)
        {
            if (!_isLeapChargeStarted)
            {
                _isLeapChargeStarted = true;
                _soundManager.PlayAmbDashReady(_heatUpStage);
                _animationScript.StartLeap();
            }

            UpdateDirection();
            return;
        }

        // Movement

        if (!_isLeapMoving) // Only for updating start movement values
        {
            _soundManager.PlayAmbDashAttack(_heatUpStage);

            _leapMoveTimer = Time.time;
            _isLeapMoving = true;

            _animationScript.PlayLeapMovement();
        }

        float travelPosition = (Time.time - _leapMoveTimer) / _leapTravelTime;

        _rb.MovePosition(Vector2.Lerp(_leapStart, _leapEnd, travelPosition));

        if (travelPosition >= 1) // Once the leap is finished
        {
            _isLeapFinished = true;
        }
    }
    #endregion

    // Returns if the player is within the leap distance of the companion (takes environment into account)
    private bool WithinLeapRange(float leapDistance)
    {
        Vector3 playerDirection = _player.transform.position - transform.position;
        Vector3 leapDirection = playerDirection.normalized;

        float targetDistance = leapDistance * _dataObj.LeapTargetTravelPercentage;
        targetDistance *= targetDistance; // Cheaper to compare square magnitudes than square roots of magnitudes

        // Check if the player is within the distance
        if ((_player.transform.position - transform.position).sqrMagnitude > targetDistance)
        {
            return false;
        }

        // Check if the player is on the other side of a wall
        RaycastHit2D wallCheck = Physics2D.Raycast(transform.position + leapDirection * 0.1f, leapDirection, leapDistance, _dataObj.EnvironmentMask); // Update layer mask variable
        if (wallCheck)
        {
            float wallDistance = (wallCheck.point - new Vector2(transform.position.x, transform.position.y)).sqrMagnitude;

            if (wallDistance < targetDistance)
            {
                return false;
            }
        }

        // Player is within the distance and not behind a wall
        return true;
    }

    #region Attacks
    // Companion will jump towards the player, damaging on collision
    /*
     * Because the leap attack is mainly movement
     * Most of the attack is handled within the fixed update loop
     */
    private void LeapAttack()
    {
        // Wait for leap to finish in fixed update
        if (!_isLeapFinished)
        {
            return;
        }

        // End leap attack
        _attackEndDelay = _leapEndTime;
        _currentState = AttackState.Delay;

        _isLeapMoving = false;
        _isLeapFinished = false;
        _isReadyToLeap = false;
    }

    // Companion will create three slow moving projectiles in front of them
    private void SpitAttack()
    {
        // Charge
        if (Time.time - _spitStartTimer <= _dataObj.SpitChargeTime)
        {
            _animationScript.SetToIdleAnimation();
            return;
        }

        // Create projectiles
        GameObject projectileRef;

        Vector3 forwardDirection = (_player.transform.position - transform.position).normalized;
        float angleFromRight = Vector3.SignedAngle(Vector3.right, forwardDirection, new Vector3(0.0f, 0.0f, 1.0f)) - _dataObj.SpitSpawnAngle;

        for (int i = 0; i < 3; i++)
        {
            // TO DO
            /*
             * Add linecast to end projectile early at wall
             */
            _animationScript.SetToSpitAnimation();

            projectileRef = _poolManager.GetFreeObject(_dataObj.SpitProjectile.name);
            projectileRef.GetComponent<CompanionLargeProjectileLogic>().Initialise(
                ref _poolManager,
                _dataObj.SpitProjectile.name,
                _dataObj.SpitProjectileLifespan,
                _spitSize, _dataObj.SpitProjectileDamage,
                transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * (angleFromRight + (i * _dataObj.SpitSpawnAngle))), Mathf.Sin(Mathf.Deg2Rad * (angleFromRight + (i * _dataObj.SpitSpawnAngle))), 0.0f) * _dataObj.SpitSpawnDistance,
                transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * (angleFromRight + (i * _dataObj.SpitSpawnAngle))), Mathf.Sin(Mathf.Deg2Rad * (angleFromRight + (i * _dataObj.SpitSpawnAngle))), 0.0f) * (_dataObj.SpitSpawnDistance + _spitTravelDistance)); 
        }

        // End spit attack
        _attackEndDelay = _dataObj.SpitEndTime;
        _currentState = AttackState.Delay;
    }

    // Companion creates a concentrated arc of projectiles towards the player
    private void LickAttack()
    {
        // Charge
        if (Time.time - _lickStartTimer <= _dataObj.LickChargeTime)
        {
            if (!_isLickStarted)
            {
                _isLickStarted = true;
                _soundManager.PlayAmbLickPrep();
                _animationScript.SetToIdleAnimation();
            }

            return;
        }

        // Each wave before the last one
        if(_lickWaveCurrentCount < _lickWaveCount - 1)
        {
            // Wait for the delay between waves
            if(Time.time - _lickLastWaveStartTime <= _lickWaveGap) { return; }

            // Create wave
            _soundManager.PlayAmbLickAttack();
            _animationScript.SetToLickAnimation();
            _animationScript.ResetActionState();
            _animationScript.ChangeAnimationTrackSpeed(Mathf.Max(_lickWaveGap / _dataObj.LickTiming, 1.0f));

            Vector3 forwardVector = (_player.transform.position - transform.position).normalized;
            Vector3 rightVector = new Vector3(forwardVector.y, -forwardVector.x, forwardVector.z);

            float forwardAngleFromRight = Vector3.SignedAngle(Vector3.right, forwardVector, new Vector3(0.0f, 0.0f, 1.0f));

            float spawnShifts = (_lickProjectiles - 1) / 2.0f;

            Vector3 startSpawnPosition = transform.position + forwardVector * _dataObj.LickProjectileSpawnDistance - rightVector * _dataObj.LickProjectileSeperationDistance * spawnShifts;

            float arcAngle = (_dataObj.LickProjectileAngle * 2.0f) / (_lickProjectiles - 1);
            Vector3 projectileDirection;
            float currentAngle;

            GameObject objectRef;

            // Creates projectiles left to right
            for (int i = 0; i < _lickProjectiles; i++)
            {
                currentAngle = _dataObj.LickProjectileAngle - arcAngle * (_lickProjectiles - 1 - i);

                projectileDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight - currentAngle)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight - currentAngle)), 0.0f) * _dataObj.ScreamProjectileSpawnDistance;

                objectRef = _poolManager.GetFreeObject(_dataObj.LickProjectile.name);

                objectRef.GetComponent<CompanionSmallProjectileLogic>().Initialise(ref _poolManager, _dataObj.LickProjectile.name,
                    startSpawnPosition + rightVector * i * _dataObj.LickProjectileSeperationDistance,
                    projectileDirection.normalized,
                    _lickProjectileSpeed, _dataObj.LickProjectileSize, _dataObj.LickProjectileDamage,
                    _dataObj.EnvironmentMask, _dataObj.PlayerMask);
            }

            _lickLastWaveStartTime = Time.time;
            _lickWaveCurrentCount++;

            return;
        }

        // Check for last wave
        if (_lickWaveCurrentCount < _lickWaveCount)
        {
            // Wait for the delay between waves
            if (Time.time - _lickLastWaveStartTime <= _lickWaveGap) { return; }

            // Create final wave
            _soundManager.PlayAmbLickAttack();
            _animationScript.SetToLickAnimation();
            _animationScript.ChangeAnimationTrackSpeed(Mathf.Max(_lickWaveGap / _dataObj.LickTiming, 1.0f));

            Vector3 forwardVector = (_player.transform.position - transform.position).normalized;
            Vector3 rightVector = new Vector3(forwardVector.y, -forwardVector.x, forwardVector.z);

            float forwardAngleFromRight = Vector3.SignedAngle(Vector3.right, forwardVector, new Vector3(0.0f, 0.0f, 1.0f));

            float spawnShifts = (_lickLastWaveProjectiles - 1) / 2.0f;

            Vector3 startSpawnPosition = transform.position + forwardVector * _dataObj.LickProjectileSpawnDistance - rightVector * _dataObj.LickProjectileSeperationDistance * spawnShifts;

            float arcAngle = (_dataObj.LickProjectileAngle * 2.0f) / (_lickLastWaveProjectiles - 1);
            Vector3 projectileDirection;
            float currentAngle;

            GameObject objectRef;

            // Creates projectiles left to right
            for (int i = 0; i < _lickLastWaveProjectiles; i++)
            {
                currentAngle = _dataObj.LickProjectileAngle - arcAngle * (_lickLastWaveProjectiles - 1 - i);

                projectileDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight - currentAngle)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight - currentAngle)), 0.0f) * _dataObj.ScreamProjectileSpawnDistance;

                objectRef = _poolManager.GetFreeObject(_dataObj.LickProjectile.name);

                objectRef.GetComponent<CompanionSmallProjectileLogic>().Initialise(ref _poolManager, _dataObj.LickProjectile.name,
                    startSpawnPosition + rightVector * i * _dataObj.LickProjectileSeperationDistance,
                    projectileDirection.normalized,
                    _lickProjectileSpeed, _dataObj.LickProjectileSize, _dataObj.LickProjectileDamage,
                    _dataObj.EnvironmentMask, _dataObj.PlayerMask);
            }

            _lickLastWaveStartTime = Time.time;
            _lickWaveCurrentCount++;

            return;
        }

        // End lick attack
        _attackEndDelay = _dataObj.LickEndTime;
        _currentState = AttackState.Delay;
    }

    // Companion creates projectiles all around them in a circle
    private void ScreamAttack()
    {
        // Charge
        if (Time.time - _screamStartTimer <= _dataObj.ScreamChargeTime)
        {
            _animationScript.StartScream();
            _animationScript.ChangeAnimationTrackSpeed(Mathf.Max(_dataObj.ScreamChargeTime / _dataObj.ScreamStartTiming, 1.0f));

            return;
        }

        // Each wave
        if(_screamWaveCurrentCount < _screamWaveCount)
        {
            // Wait for the delay between waves
            if (Time.time - _screamLastWaveStartTime <= _screamWaveGap) { return; }

            // Create wave
            _soundManager.PlayAmbScreamAttack();
            _animationScript.ContinueScream();
            _animationScript.ChangeAnimationTrackSpeed(Mathf.Max(_screamWaveGap / _dataObj.ScreamContinuedTiming, 1.0f));

            GameObject projectileRef;
            float forwardAngleFromRight = Vector3.SignedAngle(Vector3.right, _screamStartDirection, new Vector3(0.0f, 0.0f, 1.0f));
            float screamAngle = 360.0f / (float)_dataObj.NumberOfScreamProjectiles;
            Vector3 projectileSpawnPosition;

            for (int i = 0; i < _dataObj.NumberOfScreamProjectiles; i++)
            {
                projectileSpawnPosition = new Vector3(
                    Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight + screamAngle * (i + ((_screamWaveCurrentCount % 2) / 2.0f)))), 
                    Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight + screamAngle * (i + ((_screamWaveCurrentCount % 2) / 2.0f)))),
                    0.0f)
                    * _dataObj.ScreamProjectileSpawnDistance;

                projectileRef = _poolManager.GetFreeObject(_dataObj.ScreamProjectile.name);
                projectileRef.GetComponent<CompanionSmallProjectileLogic>().Initialise(ref _poolManager, _dataObj.ScreamProjectile.name,
                    transform.position + projectileSpawnPosition, projectileSpawnPosition.normalized, _screamProjectileSpeed, _dataObj.ScreamProjectileSize, _dataObj.ScreamProjectileDamage,
                    _dataObj.EnvironmentMask, _dataObj.PlayerMask);
            }

            _screamWaveCurrentCount++;
            _screamLastWaveStartTime = Time.time;

            return;
        }
        
        // End scream attack
        _attackEndDelay = _dataObj.ScreamEndTime;
        _currentState = AttackState.Delay;
    }

    // Series of leap attacks in quick succession
    private void FeralLeapAttack()
    {
        // Behaves as a leap attack
        LeapAttack();

        // Wait until leap attack is over
        if(_currentState != AttackState.Delay)
        {
            return;
        }

        // Overwrite the leap attack end values to continue the attack
        _currentState = AttackState.Feral;
        _feralLeapCurrentAmount++;

        // Check for any more leaps
        if (_feralLeapCurrentAmount < _feralLeapAmount)
        {
            // Reset leap attack
            _isReadyToLeap = false;
            _readyStartTime = Time.time;
            return;
        }

        // End feral attack as all leaps have been done
        _feralLeapCurrentAmount = 0;
        _currentState = AttackState.Delay;
    }
    #endregion

    #region Selection
    // The delay that happens between attacks
    private void Delay()
    {
        _animationScript.SetToIdleAnimation();
        _animationScript.ChangeAnimationTrackSpeed(2.0f);
        _attackEndDelay -= Time.deltaTime;
        if(_attackEndDelay < 0)
        {
            // Select an attack once delay is over
            SelectAttack();
        }
    }

    // Selects the next attack to do
    /* 
     * Also sets up attack based on stage within select attack (e.g. feral leaps will stay at stage 2 even if the companion goes to stage 3 during it)
     */
    private void SelectAttack()
    {
        UpdateDirection();
        _animationScript.ResetAnimationTrackSpeed();

        // Does a leap attack between other attacks (leap or feral)
        if (!_isLastAttackLeap)
        {
            _isLastAttackLeap = true;
            _isReadyToLeap = false;
            _readyStartTime = Time.time;

            // Check if feral leap conditions are met
            if (_leapAmount < _dataObj.LeapsBeforeFeral)
            {
                // Selecting normal leap attack
                _leapAmount++;
                _leapEndTime = _dataObj.LeapEndTime;

                switch (_heatUpStage)
                {
                    case 1:
                        _leapChargeTime = _dataObj.LeapChargeTimeStage1;
                        _leapTravelTime = _dataObj.LeapTravelTimeStage1;
                        _leapTravelDistance = _dataObj.LeapTravelDistanceStage1;
                        break;
                    case 2:
                        _leapChargeTime = _dataObj.LeapChargeTimeStage2;
                        _leapTravelTime = _dataObj.LeapTravelTimeStage2;
                        _leapTravelDistance = _dataObj.LeapTravelDistanceStage2;
                        break;
                    case 3:
                        _leapChargeTime = _dataObj.LeapChargeTimeStage3;
                        _leapTravelTime = _dataObj.LeapTravelTimeStage3;
                        _leapTravelDistance = _dataObj.LeapTravelDistanceStage3;
                        break;
                }

                _currentState = AttackState.Leap;

                return;
            }

            // Selecting feral attack
            _leapAmount = 0; // Reset leap amount for feral counter
            _leapEndTime = _dataObj.FeralLeapRestTime;

            switch (_heatUpStage)
            {
                case 1:
                    _feralLeapAmount = _dataObj.FeralLeapAmountStage1;
                    _leapChargeTime = _dataObj.FeralLeapDelayStage1;
                    _leapTravelTime = _dataObj.FeralLeapTravelTimeStage1;
                    _leapTravelDistance = _dataObj.FeralLeapDistanceStage1;
                    break;
                case 2:
                    _feralLeapAmount = _dataObj.FeralLeapAmountStage2;
                    _leapChargeTime = _dataObj.FeralLeapDelayStage2;
                    _leapTravelTime = _dataObj.FeralLeapTravelTimeStage2;
                    _leapTravelDistance = _dataObj.FeralLeapDistanceStage2;
                    break;
                case 3:
                    _feralLeapAmount = _dataObj.FeralLeapAmountStage3;
                    _leapChargeTime = _dataObj.FeralLeapDelayStage3;
                    _leapTravelTime = _dataObj.FeralLeapTravelTimeStage3;
                    _leapTravelDistance = _dataObj.FeralLeapDistanceStage3;
                    break;
            }

            _currentState = AttackState.Feral;

            return;
        }

        _isLastAttackLeap = false;

        // Check for range
        float playerDistance = (_player.transform.position - transform.position).sqrMagnitude;

        if (playerDistance <= _dataObj.CloseRangeDistance * _dataObj.CloseRangeDistance) // Check for close range
        {
            // Selecting spit attack
            _spitStartTimer = Time.time;

            switch (_heatUpStage)
            {
                case 1:
                    _spitTravelDistance = _dataObj.SpitProjectileTravelDistance1;
                    _spitSize = _dataObj.SpitProjectileSize1;
                    break;
                case 2:
                    _spitTravelDistance = _dataObj.SpitProjectileTravelDistance2;
                    _spitSize = _dataObj.SpitProjectileSize2;
                    break;
                case 3:
                    _spitTravelDistance = _dataObj.SpitProjectileTravelDistance3;
                    _spitSize = _dataObj.SpitProjectileSize3;
                    break;
            }

            _currentState = AttackState.Spit;

            return;
        }

        // Check which ranged attack
        if (_doLickAttack)
        {
            // Selecting lick attack
            _doLickAttack = false; // Next non-close range attack will be scream
            _isLickStarted = false;
            _lickStartTimer = Time.time;
            _lickWaveCurrentCount = 0;
            switch (_heatUpStage)
            {
                case 1:
                    _lickProjectileSpeed = _dataObj.LickProjectileSpeed1;
                    _lickWaveGap = _dataObj.LickWaveGapStage1;
                    _lickWaveCount = _dataObj.LickWavesStage1;
                    _lickProjectiles = _dataObj.LickProjectilesStage1;
                    _lickLastWaveProjectiles = _dataObj.LickLastWaveProjectilesStage1;
                    break;
                case 2:
                    _lickProjectileSpeed = _dataObj.LickProjectileSpeed2;
                    _lickWaveGap = _dataObj.LickWaveGapStage2;
                    _lickWaveCount = _dataObj.LickWavesStage2;
                    _lickProjectiles = _dataObj.LickProjectilesStage2;
                    _lickLastWaveProjectiles = _dataObj.LickLastWaveProjectilesStage2;
                    break;
                case 3:
                    _lickProjectileSpeed = _dataObj.LickProjectileSpeed3;
                    _lickWaveGap = _dataObj.LickWaveGapStage3;
                    _lickWaveCount = _dataObj.LickWavesStage3;
                    _lickProjectiles = _dataObj.LickProjectilesStage3;
                    _lickLastWaveProjectiles = _dataObj.LickLastWaveProjectilesStage3;
                    break;
            }

            _currentState = AttackState.Lick;

            return;
        }

        // Selecting scream attack
        _doLickAttack = true; // Next non-close range attack will be lick
        _screamStartTimer = Time.time;
        _screamWaveCurrentCount = 0;
        _screamStartDirection = (transform.position - _player.transform.position).normalized;
        switch (_heatUpStage)
        {
            case 1:
                _screamProjectileSpeed = _dataObj.ScreamProjectileSpeed1;
                _screamWaveGap = _dataObj.ScreamWaveGapStage1;
                _screamWaveCount = _dataObj.ScreamWavesStage1;
                break;
            case 2:
                _screamProjectileSpeed = _dataObj.ScreamProjectileSpeed2;
                _screamWaveGap = _dataObj.ScreamWaveGapStage2;
                _screamWaveCount = _dataObj.ScreamWavesStage2;
                break;
            case 3:
                _screamProjectileSpeed = _dataObj.ScreamProjectileSpeed3;
                _screamWaveGap = _dataObj.ScreamWaveGapStage3;
                _screamWaveCount = _dataObj.ScreamWavesStage3;
                break;
        }

        _currentState = AttackState.Scream;
    }
    #endregion

    #region Heat Up
    // Heat Up is the boss fight increasing in difficulty and goes from 1-3 (1 being the easiest and 3 being the hardest)

    // Called to increase the heat up stage
    public void HeatUp()
    {
        _heatUpStage++;
    }

    // Returns the current heat up stage
    public int GetHeatUpStage()
    {
        return _heatUpStage;
    }
    #endregion

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
