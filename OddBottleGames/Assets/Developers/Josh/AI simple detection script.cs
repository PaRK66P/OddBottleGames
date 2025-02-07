using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
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
    private List<GameObject> projectiles = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerProto");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAIRanges();
        MakeAIActions();
        BulletCleanUp();
        if (health <= 0)
        {
            DestroyAllBullets();
            Destroy(this.gameObject);
        }
    }

    void UpdateAIRanges()
    {
        Vector3 distToPlayer = this.transform.position - player.transform.position;

        float dist = distToPlayer.magnitude;

        if (dist < detectionRange)
        {
            seePlayer = true;
        }
        if (dist < shootingRange)
        {
            playerInRange = true;
        }
        else
        {
            //only reset seeplayer if they are outside the shooting range as well
            //keeps track of player through whole shooting range (in case shooting range is larger than vision range)
            if (dist > detectionRange)
            {
                seePlayer = false;
            }
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
        GameObject newBullet = Instantiate(AIProjectilePrefab);
        Vector3 toPlayer = player.transform.position - this.transform.position;
        AIProjectileScript projScript = newBullet.GetComponent<AIProjectileScript>();
        projScript.SetBulletDirectionAndSpeed(toPlayer, 8);
        newBullet.transform.position = this.transform.position + toPlayer.normalized;
        projScript.owner = this.gameObject;
        projectiles.Add(newBullet);
    }

    void BulletCleanUp()
    {
        List<GameObject> bulletsToRemove = new List<GameObject>();
        foreach (var bullet in projectiles)
        {
            if (bullet.GetComponent<AIProjectileScript>().toBeDestroyed)
            {
                bulletsToRemove.Add(bullet);
                Destroy(bullet);
            }
        }

        foreach (var bullet in bulletsToRemove)
        {
            projectiles.Remove(bullet);
        }
        bulletsToRemove.Clear();
    }

    void DestroyAllBullets()
    {
        foreach (var bullet in projectiles)
        {
            Destroy(bullet);
        }
        projectiles.Clear();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }

}
