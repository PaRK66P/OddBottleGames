using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AISimpleBehaviour : MonoBehaviour
{
    public GameObject player;
    public float detectionRange = 10;
    public float shootingRange = 6;
    public float speed = 2;
    public float fireRate = 2.0f;

    private float health = 5.0f;
    private bool seePlayer = false;
    private bool playerInRange = false;
    private float shootingTimer = 0.0f;

    public GameObject AIProjectilePrefab;
    [SerializeField]
    private List<GameObject> projectiles = new List<GameObject>();

    public ObjectPoolManager objectPoolManager;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerProto");
        objectPoolManager = GameObject.Find("GameManager").GetComponent<ObjectPoolManager>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAIVision();
        MakeAIActions();
        BulletCleanUp();
        if (health <= 0)
        {
            DestroyAllBullets();
            Destroy(this.gameObject);
        }
    }

    void UpdateAIVision()
    {
        int numRays = 20;
        float coneAngle = 250.0f;
        float angleStep = coneAngle / (float)numRays;
        bool playerBeenSeen = false;
        for (int i = 0; i < numRays+1; i++)
        {
            float angle = (-coneAngle/2.0f) + (i * angleStep);
            Vector3 dir = Quaternion.Euler(0, 0, angle) * this.transform.right;
            dir.Normalize();
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, detectionRange, LayerMask.GetMask("Player"));
            if (hit.collider != null)
            {
                Debug.DrawRay(transform.position, dir * detectionRange, Color.green);
                seePlayer = true;
                playerBeenSeen = true;
            }
            else
            {
                Debug.DrawRay(transform.position, dir * detectionRange, Color.red);
            }
        }

        Vector3 distToPlayer = this.transform.position - player.transform.position;
        float dist = distToPlayer.magnitude;
        if (!playerBeenSeen)
        {
            seePlayer = false;
            
        }
        else
        {

            if (dist < shootingRange)
            {
                playerInRange = true;
            }

            Vector3 direction = player.transform.position - this.transform.position;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float targetRotation = Mathf.LerpAngle(this.transform.rotation.eulerAngles.z, angle, Time.deltaTime * 10);
            this.transform.rotation = Quaternion.Euler(0, 0, targetRotation);
        }
        if (dist > shootingRange)
        {
            playerInRange = false;
        }
    }

    void MakeAIActions()
    {
        shootingTimer -= Time.deltaTime;
        if (seePlayer && !playerInRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            this.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        }

        if (playerInRange && shootingTimer <= 0)
        {
            shootingTimer = fireRate;
            ShootAtPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        Vector3 toPlayer = player.transform .position - this.transform.position;
        this.GetComponent<Rigidbody2D>().velocity = toPlayer.normalized * speed;
    }

    void ShootAtPlayer()
    {
        GameObject newBullet = objectPoolManager.GetFreeObject("AIProjectileProto");
        Vector3 toPlayer = player.transform.position - this.transform.position;
        AIProjectileScript projScript = newBullet.GetComponent<AIProjectileScript>();
        projScript.SetBulletDirectionAndSpeed(toPlayer, 8);
        newBullet.transform.position = this.transform.position + toPlayer.normalized;
        projScript.owner = this.gameObject;
        projectiles.Add(newBullet);
    }

    void BulletCleanUp()
    {
        List<GameObject> releasedBullets = new List<GameObject>();
        foreach (var bullet in projectiles)
        {
            if (bullet.GetComponent<AIProjectileScript>().toBeDestroyed)
            {
                objectPoolManager.ReleaseObject("AIProjectileProto", bullet);
                releasedBullets.Add(bullet);
            }
        }
        foreach (var bullet in releasedBullets)
        {
            projectiles.Remove(bullet);
        }
    }

    void DestroyAllBullets()
    {
        foreach (var bullet in projectiles)
        {
            objectPoolManager.ReleaseObject("AIProjectileProto", bullet);
        }
        projectiles.Clear();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }

}
