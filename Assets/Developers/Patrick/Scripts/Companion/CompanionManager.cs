using System.Collections;
using System.Collections.Generic;
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
        if (bossData.drawGizmos)
        {
            Vector3 forwardDirection = (playerObject.transform.position - transform.position).normalized;
            float forwardAngleFromRight = Vector3.SignedAngle(Vector3.right, forwardDirection, new Vector3(0.0f ,0.0f , 1.0f));

            Vector3 playerDirection = playerObject.transform.position - transform.position;
            Vector3 leapDirection = playerDirection.normalized;
            Vector3 leapEnd = transform.position + leapDirection * bossData.leapTravelDistance;
            if (playerDirection.sqrMagnitude >= bossData.leapTravelDistance * bossData.leapTravelDistance)
            {
                leapEnd = playerObject.transform.position;
            }
            RaycastHit2D wallCheck = Physics2D.Raycast(transform.position + leapDirection * 0.1f, leapDirection, bossData.leapTravelDistance, bossData.environmentMask); // Update layer mask variable
            if (wallCheck)
            {
                leapEnd = wallCheck.point;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, bossData.leapTravelDistance);
            Gizmos.color = new Color(1, 0.5f, 0);
            Gizmos.DrawLine(transform.position, leapEnd);
        
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * forwardAngleFromRight), Mathf.Sin(Mathf.Deg2Rad * forwardAngleFromRight), 0.0f) * bossData.spitSpawnDistance, bossData.spitProjectile.transform.localScale.x / 2.0f);
            Gizmos.DrawWireSphere(transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight + bossData.spitSpawnAngle)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight + bossData.spitSpawnAngle)), 0.0f) * bossData.spitSpawnDistance, bossData.spitProjectile.transform.localScale.x / 2.0f);
            Gizmos.DrawWireSphere(transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight - bossData.spitSpawnAngle)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight - bossData.spitSpawnAngle)), 0.0f) * bossData.spitSpawnDistance, bossData.spitProjectile.transform.localScale.x / 2.0f);

            Gizmos.DrawWireSphere(transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * forwardAngleFromRight), Mathf.Sin(Mathf.Deg2Rad * forwardAngleFromRight), 0.0f) * (bossData.spitSpawnDistance + bossData.spitProjectileTravelDistance), bossData.spitProjectile.transform.localScale.x / 2.0f);
            Gizmos.DrawWireSphere(transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight + bossData.spitSpawnAngle)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight + bossData.spitSpawnAngle)), 0.0f) * (bossData.spitSpawnDistance + bossData.spitProjectileTravelDistance), bossData.spitProjectile.transform.localScale.x / 2.0f);
            Gizmos.DrawWireSphere(transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * (forwardAngleFromRight - bossData.spitSpawnAngle)), Mathf.Sin(Mathf.Deg2Rad * (forwardAngleFromRight - bossData.spitSpawnAngle)), 0.0f) * (bossData.spitSpawnDistance + bossData.spitProjectileTravelDistance), bossData.spitProjectile.transform.localScale.x / 2.0f);

        }
    }
}
