using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class CompanionManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerObject;
    [SerializeField]
    private ObjectPoolManager poolManager;
    [SerializeField]
    private CompanionBossData bossData;
    [SerializeField]
    private CompanionFriendData friendData;

    private Rigidbody2D rb;

    private CompanionBoss bossScript;
    private CompanionFriend friendScript;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        bossScript = gameObject.AddComponent<CompanionBoss>();
        bossScript.InitialiseComponent(ref bossData, ref rb, ref playerObject, ref poolManager);

        friendScript = gameObject.AddComponent<CompanionFriend>();
    }

    // Update is called once per frame
    void Update()
    {
        bossScript.CompanionUpdate();
    }

    private void FixedUpdate()
    {
        bossScript.CompanionFixedUpdate();
    }

    private void OnDrawGizmos()
    {
        RaycastHit2D wallCheck;

        if (bossData.drawLeaps)
        {
            // Leaping
            Vector3 playerDirection = playerObject.transform.position - transform.position;
            Vector3 leapDirection = playerDirection.normalized;
            Vector3 leapEnd = transform.position + leapDirection * bossData.leapTravelDistance;
            if (playerDirection.sqrMagnitude >= bossData.leapTravelDistance * bossData.leapTravelDistance)
            {
                leapEnd = playerObject.transform.position;
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
            Vector2 forwardDirection = (playerObject.transform.position - transform.position).normalized;
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

            Vector3 forwardVector = (playerObject.transform.position - transform.position).normalized;
            Vector3 rightVector = new Vector3( forwardVector.y, -forwardVector.x, forwardVector.z);

            float forwardAngleFromRight = Vector3.SignedAngle(Vector3.right, forwardVector, new Vector3(0.0f, 0.0f, 1.0f));

            float spawnShifts = (bossData.lickProjectileNumber - 1) / 2.0f;

            Vector3 startSpawnPosition = transform.position + forwardVector * bossData.lickProjectileSpawnDistance - rightVector * bossData.lickProjectileSeperationDistance * spawnShifts;

            Vector3 projectileDirection;
            float arcAngle = (bossData.lickProjectileAngle * 2.0f) / (bossData.lickProjectileNumber - 1);
            float currentAngle = 0.0f;

            // DOESN'T WORK

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
            Vector2 forwardDirection = (playerObject.transform.position - transform.position).normalized;
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
}
