using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionBoss : MonoBehaviour
{
    enum AttackState
    {
        LEAP = 0,
        SPIT = 1,
        LICK = 2,
        SCREAM = 3,
        FERAL_LEAP = 4,
        DELAY = 5
    }

    private AttackState currentState;

    private CompanionBossData dataObj;
    private Rigidbody2D rb;
    private CompanionAnimations _animationScript;
    private PathfindingManager _pathfindingScript;
    private GameObject playerObj;
    private ObjectPoolManager poolManager;

    // Attacks
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

    // Start is called before the first frame update
    public void InitialiseComponent(ref CompanionBossData bossData, ref Rigidbody2D rigidbodyComp, ref CompanionAnimations animationScript, ref PathfindingManager pathfindingScript, ref GameObject playerObjectRef, ref ObjectPoolManager poolManagerRef)
    {
        dataObj = bossData;
        rb = rigidbodyComp;
        _animationScript = animationScript;
        _pathfindingScript = pathfindingScript;
        playerObj = playerObjectRef;
        poolManager = poolManagerRef;

        _heatUpStage = 1;
    }

    public void SetupEnemy()
    {
        currentState = AttackState.DELAY;

        _isLastAttackLeap = false;
        _leapAmount = 0;
        _doLickAttack = false; // Start with Scream

        _leapStartTimer = 0;
        _leapMoveTimer = 0;
        _isLeapMoving = false;
        _isLeapFinished = false;

        _feralLeapCurrentAmount = 0;
    }

    public void CompanionUpdate()
    {
        switch (currentState)
        {
            case AttackState.LEAP:
                LeapAttack();
                break;
            case AttackState.SPIT:
                SpitAttack();
                break;
            case AttackState.LICK:
                LickAttack();
                break;
            case AttackState.SCREAM:
                ScreamAttack();
                break;
            case AttackState.FERAL_LEAP:
                FeralLeapAttack();
                break;
            case AttackState.DELAY:
                Delay();
                break;
            default:
                Debug.LogError("Companion currentState is unknown");
                break;
        }
    }

    // Only needed for Leap movement
    public void CompanionFixedUpdate()
    {
        if (currentState != AttackState.LEAP && currentState != AttackState.FERAL_LEAP)
        {
            return;
        }

        if (!_isReadyToLeap)
        {
            Node playerNode = _pathfindingScript.NodeFromWorldPosition(playerObj.transform.position);
            Node currentNode = _pathfindingScript.NodeFromWorldPosition(transform.position);
            if (_lastPlayerNode != playerNode || _lastNode != currentNode)
            {
                _lastLeapMoveDirection = _pathfindingScript.GetPathDirection(transform.position, playerObj.transform.position);
                _lastPlayerNode = playerNode;
                _lastNode = currentNode;
            }

            rb.MovePosition(transform.position + _lastLeapMoveDirection * dataObj.moveSpeed * Time.fixedDeltaTime * Mathf.Max((dataObj.moveSpeedMultiplier * (Time.time - _readyStartTime)), 1.0f));

            if (WithinLeapRange(_leapTravelDistance))
            {
                _leapStartTimer = Time.time;
                _leapStart = transform.position; // Logically will always be in a not blocked node so this SHOULD be safe
                Vector2 playerDirection = playerObj.transform.position - transform.position;
                Vector2 leapDirection = playerDirection.normalized;

                _leapEnd = _leapStart + leapDirection * (_leapTravelDistance);
                
                // Ensures the end is not on the opposite side of a wall
                RaycastHit2D wallCheck = Physics2D.Raycast(_leapStart + leapDirection * 0.1f, leapDirection, _leapTravelDistance, dataObj.environmentMask);
                if (wallCheck)
                {
                    _leapEnd = wallCheck.point;
                }

                _leapEnd = _pathfindingScript.GetNearestNodeInDirection(_leapEnd, new Vector3(-leapDirection.x, -leapDirection.y, 0.0f)).worldPosition; // Find the nearest unblocked node along the path

                _leapStartTimer = Time.time;
                _isReadyToLeap = true;
            }

            return;
        }

        if (Time.time - _leapStartTimer <= _leapChargeTime)
        {
            _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.LEAP_CHARGE);
            return;
        }

        if (!_isLeapMoving)
        {
            _leapMoveTimer = Time.time;
            _isLeapMoving = true;

            _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.LEAP_MOVING);
        }

        float travelPosition = (Time.time - _leapMoveTimer) / _leapTravelTime;

        rb.MovePosition(Vector2.Lerp(_leapStart, _leapEnd, travelPosition));

        if (travelPosition >= 1)
        {
            _isLeapFinished = true;
        }
    }

    private bool WithinLeapRange(float leapDistance)
    {
        // Leaping
        Vector3 playerDirection = playerObj.transform.position - transform.position;
        Vector3 leapDirection = playerDirection.normalized;

        float targetDistance = leapDistance * dataObj.leapTargetTravelPercentage;
        targetDistance *= targetDistance;

        if ((playerObj.transform.position - transform.position).sqrMagnitude > targetDistance)
        {
            return false;
        }

        RaycastHit2D wallCheck = Physics2D.Raycast(transform.position + leapDirection * 0.1f, leapDirection, leapDistance, dataObj.environmentMask); // Update layer mask variable
        if (wallCheck)
        {
            float wallDistance = (wallCheck.point - new Vector2(transform.position.x, transform.position.y)).sqrMagnitude;

            if (wallDistance < targetDistance)
            {
                return false;
            }
        }
        return true;
    }

    #region Attacks

    private void LeapAttack()
    {
        // Wait for leap to finish in fixed update

        if (!_isLeapFinished)
        {
            return;
        }

        _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.LEAP_END);

        _attackEndDelay = _leapEndTime;
        currentState = AttackState.DELAY;

        _isLeapMoving = false;
        _isLeapFinished = false;
        _isReadyToLeap = false;
    }

    private void SpitAttack()
    {
        if (Time.time - _spitStartTimer <= dataObj.spitChargeTime)
        {
            _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.SPIT_CHARGE);
            return;
        }

        GameObject projectileRef;

        Vector3 forwardDirection = (playerObj.transform.position - transform.position).normalized;
        float angleFromRight = Vector3.SignedAngle(Vector3.right, forwardDirection, new Vector3(0.0f, 0.0f, 1.0f)) - dataObj.spitSpawnAngle;

        _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.SPIT_ATTACK); // Need to setup anim changes on coroutines potentially as this never stays

        for (int i = 0; i < 3; i++)
        {
            projectileRef = poolManager.GetFreeObject(dataObj.spitProjectile.name);
            projectileRef.GetComponent<CompanionLargeProjectileLogic>().Initialise(
                ref poolManager,
                dataObj.spitProjectile.name,
                dataObj.spitProjectileLifespan,
                _spitSize, dataObj.spitProjectileDamage,
                transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * (angleFromRight + (i * dataObj.spitSpawnAngle))), Mathf.Sin(Mathf.Deg2Rad * (angleFromRight + (i * dataObj.spitSpawnAngle))), 0.0f) * dataObj.spitSpawnDistance,
                transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * (angleFromRight + (i * dataObj.spitSpawnAngle))), Mathf.Sin(Mathf.Deg2Rad * (angleFromRight + (i * dataObj.spitSpawnAngle))), 0.0f) * (dataObj.spitSpawnDistance + _spitTravelDistance)); // Add linecast to end early at wall
        }

        _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.SPIT_END);

        _attackEndDelay = dataObj.spitEndTime;
        currentState = AttackState.DELAY;
    }

    private void LickAttack()
    {
        // Delay
        if (Time.time - _lickStartTimer <= dataObj.lickChargeTime)
        {
            _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.LICK_CHARGE);

            return;
        }

        if(_lickWaveCurrentCount < _lickWaveCount - 1)
        {
            if(Time.time - _lickLastWaveStartTime <= _lickWaveGap) { return; }

            Vector3 forwardVector = (playerObj.transform.position - transform.position).normalized;
            Vector3 rightVector = new Vector3(forwardVector.y, -forwardVector.x, forwardVector.z);

            float forwardAngleFromRight = Vector3.SignedAngle(Vector3.right, forwardVector, new Vector3(0.0f, 0.0f, 1.0f));

            float spawnShifts = (_lickProjectiles - 1) / 2.0f;

            Vector3 startSpawnPosition = transform.position + forwardVector * dataObj.lickProjectileSpawnDistance - rightVector * dataObj.lickProjectileSeperationDistance * spawnShifts;

            float arcAngle = (dataObj.lickProjectileAngle * 2.0f) / (_lickProjectiles - 1);
            Vector3 projectileDirection;
            float currentAngle;

            GameObject objectRef;

            _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.LICK_ATTACK);

            // Draws left to right
            for (int i = 0; i < _lickProjectiles; i++)
            {
                currentAngle = dataObj.lickProjectileAngle - arcAngle * (_lickProjectiles - 1 - i);

                projectileDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight - currentAngle)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight - currentAngle)), 0.0f) * dataObj.screamProjectileSpawnDistance;

                objectRef = poolManager.GetFreeObject(dataObj.lickProjectile.name);

                objectRef.GetComponent<CompanionSmallProjectileLogic>().Initialise(ref poolManager, dataObj.lickProjectile.name,
                    startSpawnPosition + rightVector * i * dataObj.lickProjectileSeperationDistance,
                    projectileDirection.normalized,
                    _lickProjectileSpeed, dataObj.lickProjectileSize, dataObj.lickProjectileDamage,
                    dataObj.environmentMask, dataObj.playerMask);
            }

            _lickLastWaveStartTime = Time.time;
            _lickWaveCurrentCount++;

            return;
        }

        if (_lickWaveCurrentCount < _lickWaveCount)
        {
            if (Time.time - _lickLastWaveStartTime <= _lickWaveGap) { return; }

            Vector3 forwardVector = (playerObj.transform.position - transform.position).normalized;
            Vector3 rightVector = new Vector3(forwardVector.y, -forwardVector.x, forwardVector.z);

            float forwardAngleFromRight = Vector3.SignedAngle(Vector3.right, forwardVector, new Vector3(0.0f, 0.0f, 1.0f));

            float spawnShifts = (_lickLastWaveProjectiles - 1) / 2.0f;

            Vector3 startSpawnPosition = transform.position + forwardVector * dataObj.lickProjectileSpawnDistance - rightVector * dataObj.lickProjectileSeperationDistance * spawnShifts;

            float arcAngle = (dataObj.lickProjectileAngle * 2.0f) / (_lickLastWaveProjectiles - 1);
            Vector3 projectileDirection;
            float currentAngle;

            GameObject objectRef;

            _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.LICK_ATTACK);

            // Draws left to right
            for (int i = 0; i < _lickLastWaveProjectiles; i++)
            {
                currentAngle = dataObj.lickProjectileAngle - arcAngle * (_lickLastWaveProjectiles - 1 - i);

                projectileDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight - currentAngle)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight - currentAngle)), 0.0f) * dataObj.screamProjectileSpawnDistance;

                objectRef = poolManager.GetFreeObject(dataObj.lickProjectile.name);

                objectRef.GetComponent<CompanionSmallProjectileLogic>().Initialise(ref poolManager, dataObj.lickProjectile.name,
                    startSpawnPosition + rightVector * i * dataObj.lickProjectileSeperationDistance,
                    projectileDirection.normalized,
                    _lickProjectileSpeed, dataObj.lickProjectileSize, dataObj.lickProjectileDamage,
                    dataObj.environmentMask, dataObj.playerMask);
            }

            _lickLastWaveStartTime = Time.time;
            _lickWaveCurrentCount++;

            return;
        }


        _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.LICK_END);
        _attackEndDelay = dataObj.lickEndTime;
        currentState = AttackState.DELAY;
    }

    private void ScreamAttack()
    {
        // Delay
        if (Time.time - _screamStartTimer <= dataObj.screamChargeTime)
        {
            _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.SCREAM_CHARGE);

            return;
        }


        // Firing
        if(_screamWaveCurrentCount < _screamWaveCount)
        {
            if (Time.time - _screamLastWaveStartTime <= _screamWaveGap)
            {
                return;
            }

            // Do scream
            GameObject projectileRef;
            float forwardAngleFromRight = Vector3.SignedAngle(Vector3.right, _screamStartDirection, new Vector3(0.0f, 0.0f, 1.0f));
            float screamAngle = 360.0f / (float)dataObj.numberOfScreamProjectiles;
            Vector3 projectileSpawnPosition;

            _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.SCREAM_ATTACK);

            for (int i = 0; i < dataObj.numberOfScreamProjectiles; i++)
            {
                projectileSpawnPosition = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight + screamAngle * (i + ((_screamWaveCurrentCount % 2) / 2.0f)))), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight + screamAngle * (i + ((_screamWaveCurrentCount % 2) / 2.0f)))), 0.0f) * dataObj.screamProjectileSpawnDistance;

                projectileRef = poolManager.GetFreeObject(dataObj.screamProjectile.name);
                projectileRef.GetComponent<CompanionSmallProjectileLogic>().Initialise(ref poolManager, dataObj.screamProjectile.name,
                    transform.position + projectileSpawnPosition, projectileSpawnPosition.normalized, _screamProjectileSpeed, dataObj.screamProjectileSize, dataObj.screamProjectileDamage,
                    dataObj.environmentMask, dataObj.playerMask);
            }

            _screamWaveCurrentCount++;
            _screamLastWaveStartTime = Time.time;

            return;
        }
        

        _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.SCREAM_END);

        _attackEndDelay = dataObj.screamEndTime;
        currentState = AttackState.DELAY;
    }

    private void FeralLeapAttack()
    {
        LeapAttack();

        if(currentState != AttackState.DELAY)
        {
            return;
        }

        currentState = AttackState.FERAL_LEAP;
        _feralLeapCurrentAmount++;

        if (_feralLeapCurrentAmount < _feralLeapAmount)
        {
            _isReadyToLeap = false;
            _readyStartTime = Time.time;
            return;
        }

        _feralLeapCurrentAmount = 0;
        currentState = AttackState.DELAY;
    }

    #endregion

    // Not to be caused from the player but self impossed from attacks
    private void Delay()
    {
        _attackEndDelay -= Time.deltaTime;
        if(_attackEndDelay < 0)
        {
            SelectAttack();
        }
    }

    // Also sets up attack based on stage within select attack (e.g. feral leaps will stay at stage 2 even if the companion goes to stage 3 during it)
    private void SelectAttack()
    {
        // To be updated later
        if (playerObj.transform.position.x - transform.position.x < 0)
        {
            _animationScript.ChangeAnimationDirection(CompanionAnimations.FacingDirection.LEFT);
        }
        else if (playerObj.transform.position.x - transform.position.x > 0)
        {
            _animationScript.ChangeAnimationDirection(CompanionAnimations.FacingDirection.RIGHT);
        }

        // Does a leap attack between other attacks
        if (!_isLastAttackLeap)
        {
            _isLastAttackLeap = true;
            _isReadyToLeap = false;
            _readyStartTime = Time.time;

            // Check if feral leap conditions are met
            if (_leapAmount < dataObj.leapsBeforeFeral)
            {
                _leapAmount++;
                _leapEndTime = dataObj.leapEndTime;

                switch (_heatUpStage)
                {
                    case 1:
                        _leapChargeTime = dataObj.leapChargeTimeStage1;
                        _leapTravelTime = dataObj.leapTravelTimeStage1;
                        _leapTravelDistance = dataObj.leapTravelDistanceStage1;
                        break;
                    case 2:
                        _leapChargeTime = dataObj.leapChargeTimeStage2;
                        _leapTravelTime = dataObj.leapTravelTimeStage2;
                        _leapTravelDistance = dataObj.leapTravelDistanceStage2;
                        break;
                    case 3:
                        _leapChargeTime = dataObj.leapChargeTimeStage3;
                        _leapTravelTime = dataObj.leapTravelTimeStage3;
                        _leapTravelDistance = dataObj.leapTravelDistanceStage3;
                        break;
                }

                currentState = AttackState.LEAP;

                return;
            }

            _leapAmount = 0;
            _leapEndTime = dataObj.feralLeapRestTime;

            switch (_heatUpStage)
            {
                case 1:
                    _feralLeapAmount = dataObj.feralLeapAmountStage1;
                    _leapChargeTime = dataObj.feralLeapDelayStage1;
                    _leapTravelTime = dataObj.feralLeapTravelTimeStage1;
                    _leapTravelDistance = dataObj.feralLeapDistanceStage1;
                    break;
                case 2:
                    _feralLeapAmount = dataObj.feralLeapAmountStage2;
                    _leapChargeTime = dataObj.feralLeapDelayStage2;
                    _leapTravelTime = dataObj.feralLeapTravelTimeStage2;
                    _leapTravelDistance = dataObj.feralLeapDistanceStage2;
                    break;
                case 3:
                    _feralLeapAmount = dataObj.feralLeapAmountStage3;
                    _leapChargeTime = dataObj.feralLeapDelayStage3;
                    _leapTravelTime = dataObj.feralLeapTravelTimeStage3;
                    _leapTravelDistance = dataObj.feralLeapDistanceStage3;
                    break;
            }

            currentState = AttackState.FERAL_LEAP;

            return;
        }

        _isLastAttackLeap = false;

        // Check for range
        float playerDistance = (playerObj.transform.position - transform.position).sqrMagnitude;

        if (playerDistance <= dataObj.closeRangeDistance * dataObj.closeRangeDistance)
        {
            _spitStartTimer = Time.time;

            switch (_heatUpStage)
            {
                case 1:
                    _spitTravelDistance = dataObj.spitProjectileTravelDistance1;
                    _spitSize = dataObj.spitProjectileSize1;
                    break;
                case 2:
                    _spitTravelDistance = dataObj.spitProjectileTravelDistance2;
                    _spitSize = dataObj.spitProjectileSize2;
                    break;
                case 3:
                    _spitTravelDistance = dataObj.spitProjectileTravelDistance3;
                    _spitSize = dataObj.spitProjectileSize3;
                    break;
            }

            currentState = AttackState.SPIT;

            return;
        }

        // Check which ranged attack
        if (_doLickAttack)
        {
            _doLickAttack = false;
            _lickStartTimer = Time.time;
            _lickWaveCurrentCount = 0;
            switch (_heatUpStage)
            {
                case 1:
                    _lickProjectileSpeed = dataObj.lickProjectileSpeed1;
                    _lickWaveGap = dataObj.lickWaveGapStage1;
                    _lickWaveCount = dataObj.lickWavesStage1;
                    _lickProjectiles = dataObj.lickProjectilesStage1;
                    _lickLastWaveProjectiles = dataObj.lickLastWaveProjectilesStage1;
                    break;
                case 2:
                    _lickProjectileSpeed = dataObj.lickProjectileSpeed2;
                    _lickWaveGap = dataObj.lickWaveGapStage2;
                    _lickWaveCount = dataObj.lickWavesStage2;
                    _lickProjectiles = dataObj.lickProjectilesStage2;
                    _lickLastWaveProjectiles = dataObj.lickLastWaveProjectilesStage2;
                    break;
                case 3:
                    _lickProjectileSpeed = dataObj.lickProjectileSpeed3;
                    _lickWaveGap = dataObj.lickWaveGapStage3;
                    _lickWaveCount = dataObj.lickWavesStage3;
                    _lickProjectiles = dataObj.lickProjectilesStage3;
                    _lickLastWaveProjectiles = dataObj.lickLastWaveProjectilesStage3;
                    break;
            }

            currentState = AttackState.LICK;

            return;
        }

        _doLickAttack = true;
        _screamStartTimer = Time.time;
        _screamWaveCurrentCount = 0;
        _screamStartDirection = (transform.position - playerObj.transform.position).normalized;
        switch (_heatUpStage)
        {
            case 1:
                _screamProjectileSpeed = dataObj.screamProjectileSpeed1;
                _screamWaveGap = dataObj.screamWaveGapStage1;
                _screamWaveCount = dataObj.screamWavesStage1;
                break;
            case 2:
                _screamProjectileSpeed = dataObj.screamProjectileSpeed2;
                _screamWaveGap = dataObj.screamWaveGapStage2;
                _screamWaveCount = dataObj.screamWavesStage2;
                break;
            case 3:
                _screamProjectileSpeed = dataObj.screamProjectileSpeed3;
                _screamWaveGap = dataObj.screamWaveGapStage3;
                _screamWaveCount = dataObj.screamWavesStage3;
                break;
        }

        currentState = AttackState.SCREAM;
    }

    public void HeatUp()
    {
        _heatUpStage++;
    }

    public int GetHeatUpStage()
    {
        return _heatUpStage;
    }
}
