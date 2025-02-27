using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class CompanionManager : MonoBehaviour
{
    enum CompanionStates
    {
        NONE = 0,
        ENEMY = 1,
        FRIEND = 2
    }

    [SerializeField]
    private GameObject _playerObject;
    [SerializeField]
    private ObjectPoolManager _poolManager;

    [SerializeField]
    private CompanionBossData bossData;
    [SerializeField]
    private CompanionFriendData friendData;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private CompanionCollisionDamage collisionDamageScript;
    private CompanionDetection detectionScript;

    private CompanionBoss bossScript;
    private CompanionFriend friendScript;
    private CompanionAnimations animationsScript;

    private CompanionStates _currentState;
    private float _health;

    private VisualNovelScript visualNovelManager;
    private bool hasPlayedNovel = false;

    // No protection for uninitialised Companion

    public void InitialiseEnemy(ref GameObject playerObject, ref ObjectPoolManager poolManager)
    {
        _playerObject = playerObject;
        _poolManager = poolManager;
        visualNovelManager = GameObject.Find("VisualNovelManager").GetComponent<VisualNovelScript>();


        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        if (collisionDamageScript == null)
        {
            collisionDamageScript = GetComponentInChildren<CompanionCollisionDamage>();
            collisionDamageScript.InitialiseComponent(ref friendData);
        }
        if (detectionScript == null)
        {
            detectionScript = GetComponentInChildren<CompanionDetection>();
            detectionScript.InitialiseComponent(ref friendData);
        }
        if (animationsScript == null)
        {
            animationsScript = gameObject.AddComponent<CompanionAnimations>();
            animationsScript.InitialiseComponent(ref bossData, ref friendData, ref spriteRenderer);
        }
        if (bossScript == null)
        {
            bossScript = gameObject.AddComponent<CompanionBoss>();
            bossScript.InitialiseComponent(ref bossData, ref rb, ref animationsScript, ref _playerObject, ref _poolManager);
        }
        if (friendScript == null)
        {
            friendScript = gameObject.AddComponent<CompanionFriend>();
            friendScript.InitialiseComponent(ref friendData, ref detectionScript, ref animationsScript, ref rb, ref _playerObject);
        }

        ChangeToEnemy();
    }

    // private void OnEnable()
    // {
    //     if (rb == null)
    //     {
    //         rb = GetComponent<Rigidbody2D>();
    //     }
    //     if (spriteRenderer == null)
    //     {
    //         spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    //     }
    //     if (collisionDamageScript == null)
    //     {
    //         collisionDamageScript = GetComponentInChildren<CompanionCollisionDamage>();
    //         collisionDamageScript.InitialiseComponent(ref friendData);
    //     }
    //     if (detectionScript == null)
    //     {
    //         detectionScript = GetComponentInChildren<CompanionDetection>();
    //         detectionScript.InitialiseComponent(ref friendData);
    //     }
    //     if (animationsScript == null)
    //     {
    //         animationsScript = gameObject.AddComponent<CompanionAnimations>();
    //         animationsScript.InitialiseComponent(ref bossData, ref friendData, ref spriteRenderer);
    //     }
    //     if (bossScript == null)
    //     {
    //         bossScript = gameObject.AddComponent<CompanionBoss>();
    //         bossScript.InitialiseComponent(ref bossData, ref rb, ref animationsScript, ref _playerObject, ref _poolManager);
    //     }
    //     if (friendScript == null)
    //     {
    //         friendScript = gameObject.AddComponent<CompanionFriend>();
    //         friendScript.InitialiseComponent(ref friendData, ref detectionScript, ref rb, ref _playerObject);
    //     }
    // }

    // Update is called once per frame
    void Update()
    {
        switch (_currentState)
        {
            case CompanionStates.NONE:
                break;
            case CompanionStates.ENEMY:
                bossScript.CompanionUpdate();
                break;
            case CompanionStates.FRIEND:
                friendScript.CompanionUpdate();
                break;
        }
        if (_currentState == CompanionStates.NONE && hasPlayedNovel && !visualNovelManager.isNovelSection)
        {
            switch(visualNovelManager.GetLastSelectionID())
            {
                case 0:
                {
                    ChangeToFriendly();
                    break;
                }
                case 1:
                {
                    ChangeToEnemy();
                    break;
                }
                default:
                {
                    Debug.LogError("Visual novel selection not supported. make sure to update selection code in miniboss as well as the novel that plays");
                    break;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        switch (_currentState)
        {
            case CompanionStates.NONE:
                break;
            case CompanionStates.ENEMY:
                bossScript.CompanionFixedUpdate();
                break;
            case CompanionStates.FRIEND:
                friendScript.CompanionFixedUpdate();
                break;
        }
    }

    public void TakeDamage(float damage)
    {
        if(_currentState == CompanionStates.NONE) { return; }

        _health -= damage;
        if(_health <= 0)
        {
            _currentState = CompanionStates.NONE;
            DefeatVisual();
            return;
        }

        DamageVisual();
    }

    private void DamageVisual()
    {
        StartCoroutine(DamageColor());
    }

    private void DefeatVisual()
    {
        // To be removed
        //gameObject.SetActive(false);
        if (!hasPlayedNovel)
        {
            hasPlayedNovel = true;
            visualNovelManager.StartNovelSceneByName("Miniboss tester 2");
        }
    }

    public void ChangeToNone()
    {
        _currentState = CompanionStates.NONE;

        collisionDamageScript.ChangeState(CompanionCollisionDamage.CollisionDamageStates.NONE);

        detectionScript.gameObject.SetActive(false);
    }

    public void ChangeToEnemy()
    {
        _health = bossData.health;
        bossScript.SetupEnemy();
        _currentState = CompanionStates.ENEMY;

        collisionDamageScript.ChangeState(CompanionCollisionDamage.CollisionDamageStates.PLAYER);

        detectionScript.gameObject.SetActive(true);
    }

    public void ChangeToFriendly()
    {
        _currentState = CompanionStates.FRIEND;

        collisionDamageScript.ChangeState(CompanionCollisionDamage.CollisionDamageStates.ENEMY);

        detectionScript.gameObject.SetActive(true);
    }

    IEnumerator DamageColor()
    {
        spriteRenderer.color = UnityEngine.Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = UnityEngine.Color.white;
    }

    #region Gizmos
    private void OnDrawGizmos()
    {
        if(_playerObject == null)
        {
            return;
        }

        RaycastHit2D wallCheck;

        if (bossData.drawRange)
        {
            Gizmos.color = UnityEngine.Color.cyan;
            Gizmos.DrawWireSphere(transform.position, bossData.closeRangeDistance);
        }

        if (bossData.drawLeaps)
        {
            // Leaping
            Vector3 playerDirection = _playerObject.transform.position - transform.position;
            Vector3 leapDirection = playerDirection.normalized;
            Vector3 leapEnd = transform.position + leapDirection * bossData.leapTravelDistance;
            if (playerDirection.sqrMagnitude >= bossData.leapTravelDistance * bossData.leapTravelDistance)
            {
                leapEnd = _playerObject.transform.position;
            }
            wallCheck = Physics2D.Raycast(transform.position + leapDirection * 0.1f, leapDirection, bossData.leapTravelDistance, bossData.environmentMask); // Update layer mask variable
            if (wallCheck)
            {
                leapEnd = wallCheck.point;
            }

            Gizmos.color = UnityEngine.Color.red;
            Gizmos.DrawWireSphere(transform.position, bossData.leapTravelDistance);
            Gizmos.color = new UnityEngine.Color(1, 0.5f, 0);
            Gizmos.DrawLine(transform.position, leapEnd);

        }

        if (bossData.drawSpit)
        {
            Vector2 forwardDirection = (_playerObject.transform.position - transform.position).normalized;
            float forwardAngleFromRight = Vector3.SignedAngle(Vector3.right, forwardDirection, new Vector3(0.0f, 0.0f, 1.0f));

            // Spit
            Gizmos.color = UnityEngine.Color.green;
            Gizmos.DrawWireSphere(transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * forwardAngleFromRight), Mathf.Sin(Mathf.Deg2Rad * forwardAngleFromRight), 0.0f) * bossData.spitSpawnDistance, bossData.spitProjectile.transform.localScale.x / 2.0f);
            Gizmos.DrawWireSphere(transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight + bossData.spitSpawnAngle)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight + bossData.spitSpawnAngle)), 0.0f) * bossData.spitSpawnDistance, bossData.spitProjectile.transform.localScale.x / 2.0f);
            Gizmos.DrawWireSphere(transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight - bossData.spitSpawnAngle)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight - bossData.spitSpawnAngle)), 0.0f) * bossData.spitSpawnDistance, bossData.spitProjectile.transform.localScale.x / 2.0f);

            Gizmos.DrawWireSphere(transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * forwardAngleFromRight), Mathf.Sin(Mathf.Deg2Rad * forwardAngleFromRight), 0.0f) * (bossData.spitSpawnDistance + bossData.spitProjectileTravelDistance), bossData.spitProjectile.transform.localScale.x / 2.0f);
            Gizmos.DrawWireSphere(transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight + bossData.spitSpawnAngle)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight + bossData.spitSpawnAngle)), 0.0f) * (bossData.spitSpawnDistance + bossData.spitProjectileTravelDistance), bossData.spitProjectile.transform.localScale.x / 2.0f);
            Gizmos.DrawWireSphere(transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight - bossData.spitSpawnAngle)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight - bossData.spitSpawnAngle)), 0.0f) * (bossData.spitSpawnDistance + bossData.spitProjectileTravelDistance), bossData.spitProjectile.transform.localScale.x / 2.0f);

        }

        if (bossData.drawLick)
        {
            Gizmos.color = UnityEngine.Color.yellow;

            Vector3 forwardVector = (_playerObject.transform.position - transform.position).normalized;
            Vector3 rightVector = new Vector3( forwardVector.y, -forwardVector.x, forwardVector.z);

            float forwardAngleFromRight = Vector3.SignedAngle(Vector3.right, forwardVector, new Vector3(0.0f, 0.0f, 1.0f));

            float spawnShifts = (bossData.lickProjectileNumber - 1) / 2.0f;

            Vector3 startSpawnPosition = transform.position + forwardVector * bossData.lickProjectileSpawnDistance - rightVector * bossData.lickProjectileSeperationDistance * spawnShifts;

            Vector3 projectileDirection;
            float arcAngle = (bossData.lickProjectileAngle * 2.0f) / (bossData.lickProjectileNumber - 1);
            float currentAngle = 0.0f;

            // Draws left to right
            for (int i = 0; i < bossData.lickProjectileNumber; i++)
            {
                Gizmos.DrawWireSphere(startSpawnPosition + rightVector * i * bossData.lickProjectileSeperationDistance, bossData.lickProjectile.transform.localScale.x);

                currentAngle = bossData.lickProjectileAngle - arcAngle * (bossData.lickProjectileNumber - 1 - i);

                projectileDirection = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight - currentAngle)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight - currentAngle)), 0.0f) * bossData.screamProjectileSpawnDistance;

                Gizmos.DrawLine(startSpawnPosition + rightVector * i * bossData.lickProjectileSeperationDistance, startSpawnPosition + rightVector * i * bossData.lickProjectileSeperationDistance + projectileDirection.normalized * 10);
            }
        }

        if (bossData.drawScream)
        {

            //Scream
            Gizmos.color = new UnityEngine.Color(148.0f / 255.0f, 17.0f / 255.0f, 255.0f / 255.0f);
            Vector2 forwardDirection = (_playerObject.transform.position - transform.position).normalized;
            float forwardAngleFromRight = Vector3.SignedAngle(Vector3.right, forwardDirection, new Vector3(0.0f, 0.0f, 1.0f));
            float screamAngle = 360.0f / (float) bossData.numberOfScreamProjectiles;
            Vector3 projectileSpawnPosition;
            for(int i = 0; i < bossData.numberOfScreamProjectiles; i++)
            {
                projectileSpawnPosition = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight + screamAngle * i)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight + screamAngle * i)), 0.0f) * bossData.screamProjectileSpawnDistance;
                Gizmos.DrawWireSphere(transform.position + projectileSpawnPosition, bossData.screamProjectile.transform.localScale.x);
                wallCheck = Physics2D.Raycast(transform.position + projectileSpawnPosition, projectileSpawnPosition.normalized, 100.0f, bossData.environmentMask); // Update layer mask variable
                
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
    }
    #endregion
}
