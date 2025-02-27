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
    private Vector2 _leapDirection;
    private bool _isLeapMoving;
    private bool _isLeapFinished;

    // Feral Attack
    private int _feralLeapAmount;

    // Spit Attack
    private float _spitStartTimer;

    // Lick Attack
    private float _lickStartTimer;

    // Scream Attack
    private float _screamStartTimer;

    // Start is called before the first frame update
    public void InitialiseComponent(ref CompanionBossData bossData, ref Rigidbody2D rigidbodyComp, ref GameObject _playerObjectRef, ref ObjectPoolManager poolManagerRef)
    {
        dataObj = bossData;
        rb = rigidbodyComp;
        playerObj = _playerObjectRef;
        poolManager = poolManagerRef;
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
        if (!_isLeapMoving)
        {
            return;
        }

        float travelPosition = (Time.time - _leapMoveTimer) / dataObj.leapTravelTime;

        rb.MovePosition(Vector2.Lerp(_leapStart, _leapEnd, travelPosition));

        if (travelPosition >= 1)
        {
            _isLeapFinished = true;
        }
    }

    #region Attacks

    private void LeapAttack()
    {
        if(Time.time - _leapStartTimer <= dataObj.leapChargeTime)
        {
            return;
        }

        if (!_isLeapMoving)
        {
            _leapMoveTimer = Time.time;
            _isLeapMoving = true;
        }

        // Wait for leap to finish in fixed update

        if (!_isLeapFinished)
        {
            return;
        }

        _attackEndDelay = dataObj.leapEndTime;
        currentState = AttackState.DELAY;

        _isLeapMoving = false;
        _isLeapFinished = false;
    }

    private void SpitAttack()
    {
        if (Time.time - _spitStartTimer <= dataObj.spitChargeTime)
        {
            return;
        }

        GameObject projectileRef;

        Vector3 forwardDirection = (playerObj.transform.position - transform.position).normalized;
        float angleFromRight = Vector3.SignedAngle(Vector3.right, forwardDirection, new Vector3(0.0f, 0.0f, 1.0f)) - dataObj.spitSpawnAngle;

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

        _attackEndDelay = dataObj.spitEndTime;
        currentState = AttackState.DELAY;
    }

    private void LickAttack()
    {
        if (Time.time - _lickStartTimer <= dataObj.lickChargeTime)
        {
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


        _attackEndDelay = dataObj.lickEndTime;
        currentState = AttackState.DELAY;
    }

    private void ScreamAttack()
    {
        if (Time.time - _screamStartTimer <= dataObj.screamChargeTime)
        {
            return;
        }

        // Delay
        GameObject projectileRef;
        Vector2 forwardDirection = (playerObj.transform.position - transform.position).normalized;
        float forwardAngleFromRight = Vector3.SignedAngle(Vector3.right, forwardDirection, new Vector3(0.0f, 0.0f, 1.0f));
        float screamAngle = 360.0f / (float)dataObj.numberOfScreamProjectiles;
        Vector3 projectileSpawnPosition;
        for (int i = 0; i < dataObj.numberOfScreamProjectiles; i++)
        {
            projectileSpawnPosition = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight + screamAngle * i)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight + screamAngle * i)), 0.0f) * dataObj.screamProjectileSpawnDistance;
            
            projectileRef = poolManager.GetFreeObject(dataObj.screamProjectile.name);
            projectileRef.GetComponent<CompanionSmallProjectileLogic>().Initialise(ref poolManager, dataObj.screamProjectile.name,
                transform.position + projectileSpawnPosition, projectileSpawnPosition.normalized, dataObj.screamProjectileSpeed,
                dataObj.environmentMask, dataObj.playerMask);
        }

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
            _leapStartTimer = Time.time;
            _leapStart = transform.position;
            Vector2 playerDirection = playerObj.transform.position - transform.position;
            Vector2 leapDirection = playerDirection.normalized;
            _leapEnd = _leapStart + leapDirection * dataObj.leapTravelDistance;
            if (playerDirection.sqrMagnitude >= dataObj.leapTravelDistance * dataObj.leapTravelDistance)
            {
                _leapEnd = playerObj.transform.position;
            }
            RaycastHit2D wallCheck = Physics2D.Raycast(_leapStart + leapDirection * 0.1f, leapDirection, dataObj.leapTravelDistance, dataObj.environmentMask); 
            if (wallCheck)
            {
                _leapEnd = wallCheck.point;
            }
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
        // Does a leap attack between other attacks
        if (!_isLastAttackLeap)
        {
            _isLastAttackLeap = true;

            _leapStartTimer = Time.time;
            _leapStart = transform.position;
            Vector2 playerDirection = playerObj.transform.position - transform.position;
            Vector2 leapDirection = playerDirection.normalized;
            _leapEnd = _leapStart + leapDirection * dataObj.leapTravelDistance;
            if (playerDirection.sqrMagnitude >= dataObj.leapTravelDistance * dataObj.leapTravelDistance)
            {
                _leapEnd = playerObj.transform.position;
            }
            RaycastHit2D wallCheck = Physics2D.Raycast(_leapStart + leapDirection * 0.1f, leapDirection, dataObj.leapTravelDistance, dataObj.environmentMask); 
            if (wallCheck)
            {
                _leapEnd = wallCheck.point;
                Debug.Log(wallCheck.point);
            }

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

        if( playerDistance <= dataObj.closeRangeDistance * dataObj.closeRangeDistance)
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
