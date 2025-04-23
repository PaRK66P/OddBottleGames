using System.Collections;
using UnityEngine;

public class CompanionManager : MonoBehaviour
{
    enum CompanionStates
    {
        None,
        Enemy,
        Friend,
        Idle
    }

    [Header("NECESSARY")]
    // Data
    [SerializeField]
    private CompanionBossData _bossData;
    [SerializeField]
    private CompanionFriendData _friendData;
    [SerializeField]
    private AudioSource _companionAudioSource;

    // Objects
    private GameObject _playerObject;
    private GameObject _healthbar;
    private GameObject _projectileDespawner;

    // Managers
    private ObjectPoolManager _poolManager;
    private PathfindingManager _pathfindingManager;
    private VisualNovelScript _visualNovelManager;
    private SoundManager _soundManager;

    // Components
    private Rigidbody2D _rb;
    private CompanionCollisionDamage _collisionDamageScript;
    private CompanionDetection _detectionScript;
    private CompanionManager _managerRef;
    private CompanionBoss _bossScript;
    private CompanionFriend _friendScript;
    private CompanionAnimationHandler _animationsScript;

    // Values
    private CompanionStates _currentState;
    private CompanionStates _stateToChangeTo;
    private float _health;
    private bool _isIdleTimedCoroutineRunning = false;
    private bool _hasPlayedNovel = false;

    [Header("GIZMOS")]
    // Gizmos
    [SerializeField]
    private GameObject gizmosPlayerReference;

    // Sets up the object
    /*
     * MUST BE CALLED BEFORE THIS OBJECT'S FIRST UPDATE FRAME
     */
    public void InitialiseEnemy(ref GameObject playerObject, ref ObjectPoolManager poolManager, ref PathfindingManager pathfindingManager, ref SoundManager soundManager, ref Canvas dUICanvas, ref GameObject projectileDespawner)
    {
        _managerRef = this;

        _playerObject = playerObject;
        _poolManager = poolManager;
        _pathfindingManager = pathfindingManager;
        _soundManager = soundManager;

        _projectileDespawner = projectileDespawner;

        _soundManager.SetAmbrosiaAudioSource(ref _companionAudioSource);

        _visualNovelManager = GameObject.Find("VisualNovelManager").GetComponent<VisualNovelScript>();

        _rb = GetComponent<Rigidbody2D>();

        _collisionDamageScript = GetComponentInChildren<CompanionCollisionDamage>();
        _collisionDamageScript.InitialiseComponent(ref _bossData, ref _friendData);

        _detectionScript = GetComponentInChildren<CompanionDetection>();
        _detectionScript.InitialiseComponent(ref _friendData);
        _detectionScript.GetComponent<CircleCollider2D>().radius = _friendData.DetectionRadius;

        _animationsScript = GetComponentInChildren<CompanionAnimationHandler>();
        _animationsScript.SetManager(ref _managerRef);

        _bossScript = gameObject.AddComponent<CompanionBoss>();
        _bossScript.InitialiseComponent(ref _bossData, ref _rb, ref _animationsScript, ref _pathfindingManager, ref _playerObject, ref _poolManager, ref _soundManager);

        _friendScript = gameObject.AddComponent<CompanionFriend>();
        _friendScript.InitialiseComponent(ref _friendData, ref _detectionScript, ref _animationsScript, ref _pathfindingManager, ref _soundManager, ref _rb, ref _playerObject);

        _healthbar = Instantiate(_bossData.Healthbar, dUICanvas.transform);
        _healthbar.GetComponent<UnityEngine.UI.Slider>().maxValue = _bossData.Health;
        _healthbar.GetComponent<UnityEngine.UI.Slider>().value = _bossData.Health;

        // Only created as an enemy so change to that state
        ChangeToEnemy();
        ChangeToIdleForTime(1.0f); // So they don't immediately charge the player

        // Debug
        gizmosPlayerReference = _playerObject;
    }

    #region Update
    // Update is called once per frame
    void Update()
    {
        // Runs the update function for the current state
        switch (_currentState)
        {
            case CompanionStates.None:
                break;
            case CompanionStates.Enemy:
                _bossScript.CompanionUpdate();
                break;
            case CompanionStates.Friend:
                _friendScript.CompanionUpdate();
                break;
        }
    }

    private void FixedUpdate()
    {
        // Runs the fixed update function for the current state
        switch (_currentState)
        {
            case CompanionStates.None:
                break;
            case CompanionStates.Enemy:
                _bossScript.CompanionFixedUpdate();
                break;
            case CompanionStates.Friend:
                _friendScript.CompanionFixedUpdate();
                break;
        }
    }
    #endregion

    public bool IsPlayerOnRightSide()
    {
        if (_currentState == CompanionStates.Friend)
        {
            if (_friendScript.GetTarget() != null)
            {
                return (_friendScript.GetTarget().transform.position.x - transform.position.x >= 0.0f);
            }
        }
        return (_playerObject.transform.position.x - transform.position.x >= 0.0f);
    }

    public bool IsPlayerAbove()
    {
        if (_currentState == CompanionStates.Friend)
        {
            if (_friendScript.GetTarget() != null)
            {
                return (_friendScript.GetTarget().transform.position.y - transform.position.y >= 0.0f);
            }
        }
        return (_playerObject.transform.position.y - transform.position.y >= 0.0f);
    }

    #region Damage
    public void TakeDamage(float damage)
    {
        if(_currentState != CompanionStates.Enemy) { return; } // Can only take damage in enemy state

        _animationsScript.AddHurtAnimation();

        _health -= damage;
        if(_health <= 0 && !_hasPlayedNovel)
        {
            CompanionDeath();
            return;
        }

        _soundManager.PlayAmbHit();

        // Check for heat up thresholds
        switch (_bossScript.GetHeatUpStage())
        {
            case 1:
                if(_health < _bossData.StageTwoHealthThreshold * _bossData.Health)
                {
                    _bossScript.HeatUp();
                }
                break;
            case 2:
                if(_health < _bossData.StageThreeHealthThreshold * _bossData.Health)
                {
                    _bossScript.HeatUp();
                }
                break;
            case 3:
                break;
            default:
                Debug.LogWarning("Heat up stage not accounted for: " + _bossScript.GetHeatUpStage());
                break;
        }

        _healthbar.GetComponent<UnityEngine.UI.Slider>().value = _health;
        DamageVisual();
    }

    private void CompanionDeath()
    {
        _animationsScript.ResetActionState();
        _animationsScript.ResetAnimationTrackSpeed();
        _soundManager.PlayAmbDown();
        _projectileDespawner.SetActive(true);
        ChangeToNone();
        RemoveHealthBar();
        StartCoroutine(PlayDeathEffects());
    }

    private void RemoveHealthBar()
    {
        _healthbar.SetActive(false);
    }

    private void DamageVisual()
    {
        
    }

    private void DefeatVisualNovel()
    {
        if (!_hasPlayedNovel)
        {
            _hasPlayedNovel = true;


            _visualNovelManager.StartNovelSceneByName("Ambrosia1_PortraitsDone");
            _visualNovelManager.onNovelFinish.AddListener(GetVisualNovelResult);
        }
    }

    private IEnumerator PlayDeathEffects()
    {
        // maake sure to wait for stuff to finish in this function
        // or the visual novel will start playing over the top of it

        // TO DO
        /*
         * Designate an actual death effect as this is a placeholder
         * Move any time scale changes to the time manager (might be redundant if the effect changes)
         */

        float targetTime = 0.5f;
        while (Time.timeScale > targetTime)
        {
            Time.timeScale -= Time.unscaledDeltaTime;
            if (Time.timeScale < targetTime)
            {
                Time.timeScale = targetTime;
            }
            yield return null;
        }


        DefeatVisualNovel();
        //yield return null;
    }
    #endregion

    #region State Management
    public void ChangeToNone()
    {
        if (WaitForIdle(CompanionStates.None))
        {
            return;
        }

        _currentState = CompanionStates.None;

        _collisionDamageScript.ChangeState(CompanionCollisionDamage.CollisionDamageStates.None);

        _detectionScript.gameObject.SetActive(false);
    }

    public void ChangeToEnemy()
    {
        if (WaitForIdle(CompanionStates.Enemy))
        {
            return;
        }

        _health = _bossData.Health;
        _bossScript.SetupEnemy();
        _currentState = CompanionStates.Enemy;

        CompanionManager companionManager = this;
        _playerObject.GetComponent<PlayerManager>().SetAllyCompanion(false, ref companionManager);

        _collisionDamageScript.ChangeState(CompanionCollisionDamage.CollisionDamageStates.Player);

        _detectionScript.gameObject.SetActive(false);
    }

    public void ChangeToFriendly()
    {
        if (WaitForIdle(CompanionStates.Friend))
        {
            return;
        }

        _currentState = CompanionStates.Friend;
        CompanionManager companionManager = this;
        _playerObject.GetComponent<PlayerManager>().SetAllyCompanion(true, ref companionManager);

        _collisionDamageScript.ChangeState(CompanionCollisionDamage.CollisionDamageStates.Enemy);

        _detectionScript.gameObject.SetActive(true);

        // Quick fix as will never change from friendly->enemy
        _rb.excludeLayers = _friendData.PlayerAttacksLayer;
    }

    // Updates the Idle state for when it's over to change to the new state
    private bool WaitForIdle(CompanionStates newState)
    {
        if(_currentState != CompanionStates.Idle)
        {
            return false;
        }

        _stateToChangeTo = newState;

        return true;
    }

    public void ChangeToIdleForTime(float timeForIdle)
    {
        // TO DO
        /*
         * Check if the idle for time is shorter than the remaining idle time
         * If so then don't do anything (similar to time manager)
         */

        if(_isIdleTimedCoroutineRunning) // Need to idle for a different amount
        {
            StopCoroutine("SetIdle");
        }

        _stateToChangeTo = _currentState;
        _currentState = CompanionStates.Idle;

        StartCoroutine("SetIdle", timeForIdle);
    }

    private IEnumerator SetIdle(float idleTime)
    {
        _isIdleTimedCoroutineRunning = true;

        yield return new WaitForSeconds(idleTime);

        _currentState = _stateToChangeTo;
        switch (_stateToChangeTo)
        {
            case CompanionStates.None:
                ChangeToNone();
                break;
            case CompanionStates.Enemy:
                ChangeToEnemy();
                break;
            case CompanionStates.Friend:
                ChangeToFriendly();
                break;
        }

        _isIdleTimedCoroutineRunning = false;
    }
    public bool IsFriendly()
    {
        return _currentState == CompanionStates.Friend;
    }

    #endregion

    #region Friendly Companion Communication
    // Sets the target that the player is currently fighting for the companion to also target them
    public void SetPlayerTarget(GameObject target)
    {
        _friendScript.SetPlayerTarget(target);
    }

    // Removes the target the player is currently fighting
    public void RemovePlayerTarget(GameObject target)
    {
        _friendScript.RemovePlayerTarget(target);
    }

    // Just a direct move, be careful when calling
    public void TeleportToPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
    #endregion

    private void GetVisualNovelResult()
    {
        switch (_visualNovelManager.GetLastSelectionID())
        {
            case 0:
            case 1:
            case 3:
            case 5:
            case 7:
            case 9:
            case 11:
            case 13:
            case 15:
            case 17:
            case 19:
            case 21:
            case 25:
                _projectileDespawner.SetActive(false);
                gameObject.GetComponent<enemyScr>().DecreaseEnemyCount();
                ChangeToFriendly();
                _playerObject.GetComponent<PlayerManager>().SetAllyCompanion(true, ref _managerRef);
                break;
            case 2:
            case 4:
            case 6:
            case 8:
            case 10:
            case 12:
            case 14:
            case 16:
            case 18:
            case 20:
            case 22:
            case 23:
            case 24:
            case 26:
            case 27:
            case 28:
                _projectileDespawner.SetActive(false);
                _soundManager.PlayAmbConsume();
                _playerObject.GetComponent<PlayerManager>().EvolveDash(true);
                gameObject.GetComponent<enemyScr>().releaseEnemy();
                break;
            default:
                _soundManager.PlayAmbConsume();
                _playerObject.GetComponent<PlayerManager>().EvolveDash(true);
                gameObject.GetComponent<enemyScr>().releaseEnemy();
                Debug.LogError("Visual novel selection of " + _visualNovelManager.GetLastSelectionID() + " not supported. make sure to update selection code in miniboss as well as the novel that plays");
                break;
        }
        _visualNovelManager.onNovelFinish.RemoveListener(GetVisualNovelResult);
    }

    #region Gizmos
    private void OnDrawGizmos()
    {
        if (_bossData.DrawRange)
        {
            Gizmos.color = UnityEngine.Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _bossData.CloseRangeDistance);
        }

        if (_friendData.DrawDetectionRange)
        {
            Gizmos.color = new UnityEngine.Color(1.0f, 0.753f, 0.796f);
            Gizmos.DrawWireSphere(transform.position, _friendData.DetectionRadius);
        }

        // Draw Companion
        if (_bossData.DrawLeap || _bossData.DrawFeralLeap)
        {
            GizmoDrawCompanion(transform.position);
        }

        // Gizmos that require the player
        if (gizmosPlayerReference == null)
        {
            return;
        }

        RaycastHit2D wallCheck;

        if (_bossData.DrawLeap)
        {
            // Leaping
            float leapTravelDistance = 0.0f;

            switch (_bossData.HeatUpStage)
            {
                case 1:
                    leapTravelDistance = _bossData.LeapTravelDistanceStage1;
                    break;
                case 2:
                    leapTravelDistance = _bossData.LeapTravelDistanceStage2;
                    break;
                case 3:
                    leapTravelDistance = _bossData.LeapTravelDistanceStage3;
                    break;
            }

            Vector3 playerDirection = gizmosPlayerReference.transform.position - transform.position;
            Vector3 leapDirection = playerDirection.normalized;
            Vector3 leapEnd = transform.position + leapDirection * leapTravelDistance;

            Vector3 targetPosition = transform.position + leapDirection * leapTravelDistance * _bossData.LeapTargetTravelPercentage;
            float targetDistance = (targetPosition - transform.position).sqrMagnitude;
            bool drawTarget = true;

            if ((gizmosPlayerReference.transform.position - transform.position).sqrMagnitude > targetDistance)
            {
                drawTarget = false;
            }

            if (playerDirection.sqrMagnitude < (leapTravelDistance * leapTravelDistance * _bossData.LeapTargetTravelPercentage) * (leapTravelDistance * leapTravelDistance * _bossData.LeapTargetTravelPercentage))
            {
                targetPosition = gizmosPlayerReference.transform.position;
                targetDistance = (targetPosition - transform.position).sqrMagnitude;

            }

            wallCheck = Physics2D.Raycast(transform.position + leapDirection * 0.1f, leapDirection, leapTravelDistance, _bossData.EnvironmentMask); // Update layer mask variable
            if (wallCheck)
            {
                float wallDistance = (wallCheck.point - new Vector2(transform.position.x, transform.position.y)).sqrMagnitude;

                leapEnd = wallCheck.point;

                if (wallDistance < targetDistance)
                {
                    drawTarget = false;
                }
            }

            Gizmos.color = UnityEngine.Color.red;
            Gizmos.DrawWireSphere(transform.position, leapTravelDistance * _bossData.LeapTargetTravelPercentage);

            Gizmos.color = new UnityEngine.Color(1, 0.5f, 0);
            Gizmos.DrawLine(transform.position, leapEnd);
            if (drawTarget)
            {
                GizmoDrawCompanion(leapEnd);
            }

        }

        if (_bossData.DrawFeralLeap)
        {
            // Leaping
            float leapTravelDistance = 0.0f;

            switch (_bossData.HeatUpStage)
            {
                case 1:
                    leapTravelDistance = _bossData.FeralLeapDistanceStage1;
                    break;
                case 2:
                    leapTravelDistance = _bossData.FeralLeapDistanceStage2;
                    break;
                case 3:
                    leapTravelDistance = _bossData.FeralLeapDistanceStage3;
                    break;
            }

            Vector3 playerDirection = gizmosPlayerReference.transform.position - transform.position;
            Vector3 leapDirection = playerDirection.normalized;
            Vector3 leapEnd = transform.position + leapDirection * leapTravelDistance;

            Vector3 targetPosition = transform.position + leapDirection * leapTravelDistance * _bossData.LeapTargetTravelPercentage;
            float targetDistance = (targetPosition - transform.position).sqrMagnitude;
            bool drawTarget = true;

            if ((gizmosPlayerReference.transform.position - transform.position).sqrMagnitude > targetDistance)
            {
                drawTarget = false;
            }

            if (playerDirection.sqrMagnitude < (leapTravelDistance * leapTravelDistance * _bossData.LeapTargetTravelPercentage) * (leapTravelDistance * leapTravelDistance * _bossData.LeapTargetTravelPercentage))
            {
                targetPosition = gizmosPlayerReference.transform.position;
                targetDistance = (targetPosition - transform.position).sqrMagnitude;

            }

            wallCheck = Physics2D.Raycast(transform.position + leapDirection * 0.1f, leapDirection, leapTravelDistance, _bossData.EnvironmentMask); // Update layer mask variable
            if (wallCheck)
            {
                float wallDistance = (wallCheck.point - new Vector2(transform.position.x, transform.position.y)).sqrMagnitude;

                leapEnd = wallCheck.point;

                if (wallDistance < targetDistance)
                {
                    drawTarget = false;
                }
            }
            Gizmos.color = UnityEngine.Color.yellow;
            Gizmos.DrawWireSphere(transform.position, leapTravelDistance * _bossData.LeapTargetTravelPercentage);

            Gizmos.color = new UnityEngine.Color(1, 1, 1);
            Gizmos.DrawLine(transform.position, leapEnd);
            if (drawTarget)
            {
                GizmoDrawCompanion(leapEnd);
            }
        }

        if (_bossData.DrawSpit)
        {
            float spitProjectileTravelDistance = 0.0f;

            switch (_bossData.HeatUpStage)
            {
                case 1:
                    spitProjectileTravelDistance = _bossData.SpitProjectileTravelDistance1;
                    break;
                case 2:
                    spitProjectileTravelDistance = _bossData.SpitProjectileTravelDistance2;
                    break;
                case 3:
                    spitProjectileTravelDistance = _bossData.SpitProjectileTravelDistance3;
                    break;
            }

            Vector2 forwardDirection = (gizmosPlayerReference.transform.position - transform.position).normalized;
            float forwardAngleFromRight = Vector3.SignedAngle(Vector3.right, forwardDirection, new Vector3(0.0f, 0.0f, 1.0f));

            // Spit
            Gizmos.color = UnityEngine.Color.green;
            Gizmos.DrawWireSphere(transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * forwardAngleFromRight), Mathf.Sin(Mathf.Deg2Rad * forwardAngleFromRight), 0.0f) * _bossData.SpitSpawnDistance, _bossData.SpitProjectile.transform.localScale.x / 2.0f);
            Gizmos.DrawWireSphere(transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight + _bossData.SpitSpawnAngle)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight + _bossData.SpitSpawnAngle)), 0.0f) * _bossData.SpitSpawnDistance, _bossData.SpitProjectile.transform.localScale.x / 2.0f);
            Gizmos.DrawWireSphere(transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight - _bossData.SpitSpawnAngle)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight - _bossData.SpitSpawnAngle)), 0.0f) * _bossData.SpitSpawnDistance, _bossData.SpitProjectile.transform.localScale.x / 2.0f);

            Gizmos.DrawWireSphere(transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * forwardAngleFromRight), Mathf.Sin(Mathf.Deg2Rad * forwardAngleFromRight), 0.0f) * (_bossData.SpitSpawnDistance + spitProjectileTravelDistance), _bossData.SpitProjectile.transform.localScale.x / 2.0f);
            Gizmos.DrawWireSphere(transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight + _bossData.SpitSpawnAngle)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight + _bossData.SpitSpawnAngle)), 0.0f) * (_bossData.SpitSpawnDistance + spitProjectileTravelDistance), _bossData.SpitProjectile.transform.localScale.x / 2.0f);
            Gizmos.DrawWireSphere(transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight - _bossData.SpitSpawnAngle)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight - _bossData.SpitSpawnAngle)), 0.0f) * (_bossData.SpitSpawnDistance + spitProjectileTravelDistance), _bossData.SpitProjectile.transform.localScale.x / 2.0f);

        }

        if (_bossData.DrawLick)
        {
            int lickProjectileNumber = 0;

            switch (_bossData.HeatUpStage)
            {
                case 1:
                    lickProjectileNumber = _bossData.LickLastWaveProjectilesStage1;
                    break;
                case 2:
                    lickProjectileNumber = _bossData.LickLastWaveProjectilesStage1;
                    break;
                case 3:
                    lickProjectileNumber = _bossData.LickLastWaveProjectilesStage1;
                    break;
            }

            Gizmos.color = UnityEngine.Color.yellow;

            Vector3 forwardVector = (gizmosPlayerReference.transform.position - transform.position).normalized;
            Vector3 rightVector = new Vector3(forwardVector.y, -forwardVector.x, forwardVector.z);

            float forwardAngleFromRight = Vector3.SignedAngle(Vector3.right, forwardVector, new Vector3(0.0f, 0.0f, 1.0f));

            float spawnShifts = (lickProjectileNumber - 1) / 2.0f;

            Vector3 startSpawnPosition = transform.position + forwardVector * _bossData.LickProjectileSpawnDistance - rightVector * _bossData.LickProjectileSeperationDistance * spawnShifts;

            Vector3 projectileDirection;
            float arcAngle = (_bossData.LickProjectileAngle * 2.0f) / (lickProjectileNumber - 1);
            float currentAngle = 0.0f;

            // Draws left to right
            for (int i = 0; i < lickProjectileNumber; i++)
            {
                Gizmos.DrawWireSphere(startSpawnPosition + rightVector * i * _bossData.LickProjectileSeperationDistance, _bossData.LickProjectile.transform.localScale.x);

                currentAngle = _bossData.LickProjectileAngle - arcAngle * (lickProjectileNumber - 1 - i);

                projectileDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight - currentAngle)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight - currentAngle)), 0.0f) * _bossData.ScreamProjectileSpawnDistance;

                Gizmos.DrawLine(startSpawnPosition + rightVector * i * _bossData.LickProjectileSeperationDistance, startSpawnPosition + rightVector * i * _bossData.LickProjectileSeperationDistance + projectileDirection.normalized * 10);
            }
        }

        if (_bossData.DrawScream)
        {

            //Scream
            Gizmos.color = new UnityEngine.Color(148.0f / 255.0f, 17.0f / 255.0f, 255.0f / 255.0f);
            Vector2 forwardDirection = (gizmosPlayerReference.transform.position - transform.position).normalized;
            float forwardAngleFromRight = Vector3.SignedAngle(Vector3.right, forwardDirection, new Vector3(0.0f, 0.0f, 1.0f));
            float screamAngle = 360.0f / (float) _bossData.NumberOfScreamProjectiles;
            Vector3 projectileSpawnPosition;
            for(int i = 0; i < _bossData.NumberOfScreamProjectiles; i++)
            {
                projectileSpawnPosition = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight + screamAngle * i)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight + screamAngle * i)), 0.0f) * _bossData.ScreamProjectileSpawnDistance;
                Gizmos.DrawWireSphere(transform.position + projectileSpawnPosition, _bossData.ScreamProjectile.transform.localScale.x);
                wallCheck = Physics2D.Raycast(transform.position + projectileSpawnPosition, projectileSpawnPosition.normalized, 100.0f, _bossData.EnvironmentMask); // Update layer mask variable
                
                if (wallCheck)
                {
                    Gizmos.DrawLine(transform.position + projectileSpawnPosition, wallCheck.point);
                }
                else
                {
                    Gizmos.DrawLine(transform.position + projectileSpawnPosition, projectileSpawnPosition.normalized * 100.0f);
                }
            }
        }

        if (_friendData.DrawIdleDistance)
        {
            Gizmos.color = UnityEngine.Color.green;
            Gizmos.DrawWireSphere(gizmosPlayerReference.transform.position, _friendData.IdleDistance);
        }
    }

    private void GizmoDrawCompanion(Vector3 drawPosition)
    {
        UnityEngine.Color originalColor = Gizmos.color;
        Gizmos.color = new UnityEngine.Color(1.0f, 0.7f, 0.6f);
        Gizmos.DrawWireCube(drawPosition + new Vector3(_bossData.WidthOffset, _bossData.HeightOffset, 0.0f), new Vector3(_bossData.CompanionWidth, _bossData.CompanionHeight, 0.0f));
        Gizmos.color = originalColor;
    }
    #endregion
}
