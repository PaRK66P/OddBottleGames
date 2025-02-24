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
        STUNNED = 5
    }

    private AttackState currentState;

    private CompanionBossData dataObj;
    private Rigidbody2D rb;
    private GameObject playerObj;

    // Attacks
    private float _attackEndDelay;
    private bool _isLastAttackLeap;
    private int _leapAmount;
    private bool _doLickAttack;

    // Leap Attack
    private float _leapStartTimer;
    private float _leapMoveTimer;
    private Vector2 _leapStart;
    private Vector2 _leapDirection;
    private bool _isLeapMoving;
    private bool _isLeapFinished;

    // Start is called before the first frame update
    public void InitialiseComponent(ref CompanionBossData bossData, ref Rigidbody2D rigidbodyComp, ref GameObject playerObjectRef)
    {
        dataObj = bossData;
        rb = rigidbodyComp;
        playerObj = playerObjectRef;

        currentState = AttackState.LEAP;

        _isLastAttackLeap = true;
        _leapAmount = 1;
        _doLickAttack = false; // Start with Scream

        _leapStartTimer = 0;
        _leapMoveTimer = 0;
        _isLeapMoving = false;
        _isLeapFinished = false;
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
            case AttackState.STUNNED:
                StunnedDelay();
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

        rb.MovePosition(Vector2.Lerp(_leapStart, _leapStart + _leapDirection * dataObj.leapTravelDistance, travelPosition));

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

            _leapStart = transform.position;
            _leapDirection = (playerObj.transform.position - transform.position).normalized;
        }

        // Wait for leap to finish in fixed update

        if (!_isLeapFinished)
        {
            return;
        }

        _attackEndDelay = dataObj.leapEndTime;
        currentState = AttackState.STUNNED;

        _isLeapMoving = false;
        _isLeapFinished = false;

        Debug.Log("Leap");
    }

    private void SpitAttack()
    {
        Debug.Log("Spit");

        _attackEndDelay = 0.0f;
        currentState = AttackState.STUNNED;
    }

    private void LickAttack()
    {
        Debug.Log("Lick");

        _attackEndDelay = 0.0f;
        currentState = AttackState.STUNNED;
    }

    private void ScreamAttack()
    {
        Debug.Log("Scream");

        _attackEndDelay = 0.0f;
        currentState = AttackState.STUNNED;
    }

    private void FeralLeapAttack()
    {
        Debug.Log("Feral");

        _attackEndDelay = 0.0f;
        currentState = AttackState.STUNNED;
    }

    #endregion

    // Not to be caused from the player but self impossed from attacks
    private void StunnedDelay()
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

            return;
        }

        // Check which ranged attack
        if (_doLickAttack)
        {
            currentState = AttackState.LICK;

            _doLickAttack = false;

            return;
        }

        _doLickAttack = true;

        currentState = AttackState.SCREAM;


    }
}
