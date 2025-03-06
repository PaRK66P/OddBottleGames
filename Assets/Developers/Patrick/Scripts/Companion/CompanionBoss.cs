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

    // Feral Attack
    private int _feralLeapAmount;

    // Spit Attack
    private float _spitStartTimer;

    // Lick Attack
    private float _lickStartTimer;

    // Scream Attack
    private float _screamStartTimer;

    // Start is called before the first frame update
    public void InitialiseComponent(ref CompanionBossData bossData, ref Rigidbody2D rigidbodyComp, ref CompanionAnimations animationScript, ref PathfindingManager pathfindingScript, ref GameObject playerObjectRef, ref ObjectPoolManager poolManagerRef)
    {
        dataObj = bossData;
        rb = rigidbodyComp;
        _animationScript = animationScript;
        _pathfindingScript = pathfindingScript;
        playerObj = playerObjectRef;
        poolManager = poolManagerRef;
        //Debug.Log("poolManager" + poolManagerRef.name);
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

        _feralLeapAmount = 0;
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

            if (WithinLeapRange(dataObj.leapTravelDistance + (currentState == AttackState.FERAL_LEAP ? dataObj.feralLeapAdditionalDistance : 0.0f)))
            {
                _leapStartTimer = Time.time;
                _leapStart = transform.position;
                Vector2 playerDirection = playerObj.transform.position - transform.position;
                Vector2 leapDirection = playerDirection.normalized;
                _leapEnd = _leapStart + leapDirection * (dataObj.leapTravelDistance + (currentState == AttackState.FERAL_LEAP ? dataObj.feralLeapAdditionalDistance : 0.0f));
                if (playerDirection.sqrMagnitude >= (dataObj.leapTravelDistance + (currentState == AttackState.FERAL_LEAP ? dataObj.feralLeapAdditionalDistance : 0.0f)) * dataObj.leapTravelDistance)
                {
                    _leapEnd = playerObj.transform.position;
                }
                RaycastHit2D wallCheck = Physics2D.Raycast(_leapStart + leapDirection * 0.1f, leapDirection, dataObj.leapTravelDistance + (currentState == AttackState.FERAL_LEAP ? dataObj.feralLeapAdditionalDistance : 0.0f), dataObj.environmentMask);
                if (wallCheck)
                {
                    _leapEnd = wallCheck.point;
                }

                _leapStartTimer = Time.time;
                _isReadyToLeap = true;
            }

            return;
        }

        if (Time.time - _leapStartTimer <= dataObj.leapChargeTime)
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

        float travelPosition = (Time.time - _leapMoveTimer) / dataObj.leapTravelTime;

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

        Vector3 targetPosition = transform.position + leapDirection * leapDistance * dataObj.leapTargetTravelPercentage;
        float targetDistance = (targetPosition - transform.position).sqrMagnitude;

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

        _attackEndDelay = dataObj.leapEndTime;
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
                transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * (angleFromRight + (i * dataObj.spitSpawnAngle))), Mathf.Sin(Mathf.Deg2Rad * (angleFromRight + (i * dataObj.spitSpawnAngle))), 0.0f) * dataObj.spitSpawnDistance,
                transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * (angleFromRight + (i * dataObj.spitSpawnAngle))), Mathf.Sin(Mathf.Deg2Rad * (angleFromRight + (i * dataObj.spitSpawnAngle))), 0.0f) * (dataObj.spitSpawnDistance + dataObj.spitProjectileTravelDistance)); // Add linecast to end early at wall
        }

        _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.SPIT_END);

        _attackEndDelay = dataObj.spitEndTime;
        currentState = AttackState.DELAY;
    }

    private void LickAttack()
    {
        if (Time.time - _lickStartTimer <= dataObj.lickChargeTime)
        {
            _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.LICK_CHARGE);

            return;
        }

        Vector3 forwardVector = (playerObj.transform.position - transform.position).normalized;
        Vector3 rightVector = new Vector3(forwardVector.y, -forwardVector.x, forwardVector.z);

        float forwardAngleFromRight = Vector3.SignedAngle(Vector3.right, forwardVector, new Vector3(0.0f, 0.0f, 1.0f));

        float spawnShifts = (dataObj.lickProjectileNumber - 1) / 2.0f;

        Vector3 startSpawnPosition = transform.position + forwardVector * dataObj.lickProjectileSpawnDistance - rightVector * dataObj.lickProjectileSeperationDistance * spawnShifts;

        float arcAngle = (dataObj.lickProjectileAngle * 2.0f) / (dataObj.lickProjectileNumber - 1);
        Vector3 projectileDirection;
        float currentAngle;

        GameObject objectRef;

        _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.LICK_ATTACK);

        // Draws left to right
        for (int i = 0; i < dataObj.lickProjectileNumber; i++)
        {
            currentAngle = dataObj.lickProjectileAngle - arcAngle * (dataObj.lickProjectileNumber - 1 - i);

            projectileDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight - currentAngle)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight - currentAngle)), 0.0f) * dataObj.screamProjectileSpawnDistance;

            objectRef = poolManager.GetFreeObject(dataObj.lickProjectile.name);

            objectRef.GetComponent<CompanionSmallProjectileLogic>().Initialise(ref poolManager, dataObj.lickProjectile.name,
                startSpawnPosition + rightVector * i * dataObj.lickProjectileSeperationDistance,
                projectileDirection.normalized,
                dataObj.lickProjectileSpeed,
                dataObj.environmentMask, dataObj.playerMask);
        }

        _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.LICK_END);
        _attackEndDelay = dataObj.lickEndTime;
        currentState = AttackState.DELAY;
    }

    private void ScreamAttack()
    {
        if (Time.time - _screamStartTimer <= dataObj.screamChargeTime)
        {
            _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.SCREAM_CHARGE);

            return;
        }

        // Delay
        GameObject projectileRef;
        Vector2 forwardDirection = (playerObj.transform.position - transform.position).normalized;
        float forwardAngleFromRight = Vector3.SignedAngle(Vector3.right, forwardDirection, new Vector3(0.0f, 0.0f, 1.0f));
        float screamAngle = 360.0f / (float)dataObj.numberOfScreamProjectiles;
        Vector3 projectileSpawnPosition;

        _animationScript.ChangeAnimationState(CompanionAnimations.AnimationState.SCREAM_ATTACK);

        for (int i = 0; i < dataObj.numberOfScreamProjectiles; i++)
        {
            projectileSpawnPosition = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight + screamAngle * i)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight + screamAngle * i)), 0.0f) * dataObj.screamProjectileSpawnDistance;
            
            projectileRef = poolManager.GetFreeObject(dataObj.screamProjectile.name);
            projectileRef.GetComponent<CompanionSmallProjectileLogic>().Initialise(ref poolManager, dataObj.screamProjectile.name,
                transform.position + projectileSpawnPosition, projectileSpawnPosition.normalized, dataObj.screamProjectileSpeed,
                dataObj.environmentMask, dataObj.playerMask);
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
        _feralLeapAmount++;

        if (_feralLeapAmount < dataObj.feralLeapAmount)
        {
            _isReadyToLeap = false;
            _readyStartTime = Time.time;
            return;
        }

        _feralLeapAmount = 0;
        _attackEndDelay = dataObj.feralLeapEndTime;
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
                currentState = AttackState.LEAP;

                _leapAmount++;

                return;
            }

            _leapAmount = 0;
            currentState = AttackState.FERAL_LEAP;

            return;
        }

        _isLastAttackLeap = false;

        // Check for range
        float playerDistance = (playerObj.transform.position - transform.position).sqrMagnitude;

        if (playerDistance <= dataObj.closeRangeDistance * dataObj.closeRangeDistance)
        {
            currentState = AttackState.SPIT;

            _spitStartTimer = Time.time;

            return;
        }

        // Check which ranged attack
        if (_doLickAttack)
        {
            currentState = AttackState.LICK;

            _doLickAttack = false;
            _lickStartTimer = Time.time;

            return;
        }

        _doLickAttack = true;
        _screamStartTimer = Time.time;

        currentState = AttackState.SCREAM;
    }
}
