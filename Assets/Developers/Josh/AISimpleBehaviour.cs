using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using TMPro;
using UnityEngine;

public enum AIType
{
    SHOOTER,
    BLASTER,
    SPRAYER
}
public class AISimpleBehaviour : MonoBehaviour
{
    public GameObject colliderChild;
    public GameObject player;
    public GameObject particle;
    public float detectionRange = 15;
    public float shootingRange = 9;
    public float speed = 2;
    public float fireRate = 2.0f;
    public float maxHealth = 6.0f;
    [Range(0.0f, 1.0f)] public float hitStunLength = 0.2f;

    private float health = 6.0f;
    private bool seePlayer = true;
    private bool playerInRange = false;
    private float shootingTimer = 0.0f;
    private float hitStunTimer = 1.0f;
    
    private bool inStun = false;
    private bool isDead = false;
    private float deadTimer = 0.0f;
    public bool takesKnockback = true;

    public GameObject AIProjectilePrefab;

    public ObjectPoolManager objectPoolManager;

    public AIType aiMode = AIType.SHOOTER;

    private LilGuysAnimationHandler _animations;

    public void Instantiate(ref ObjectPoolManager bObjectPoolManager, ref GameObject bPlayer)
    {
        isDead = false;
        shootingTimer = 0.0f;
        deadTimer = 0.0f;
        health = maxHealth;
        inStun = false;
        objectPoolManager = bObjectPoolManager;
        player = bPlayer;

        _animations = GetComponentInChildren<LilGuysAnimationHandler>();
        _animations.AddIdle();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            if (hitStunTimer < hitStunLength)
            {
                hitStunTimer += Time.deltaTime;
                inStun = true;
                GetComponent<Rigidbody2D>().velocity *= 0.96f;
            }
            else
            {
                inStun = false;
            }
            if (!inStun)
            {
                UpdateAIVision();
                MakeAIActions();
            }
        }
        if (health <= 0)
        {
            GetComponent<CompanionTargettingHandler>().ReleaseAsTarget();
            colliderChild.SetActive(false);
            isDead = true;
            OnDeath();
        }
    }

    void UpdateAIVision()
    {
        Vector3 distToPlayer = this.transform.position - player.transform.position;
        float dist = distToPlayer.magnitude;
        if (dist < shootingRange)
        {
            playerInRange = true;
        }
        else 
        {
            playerInRange = false;
        }

        if (dist < detectionRange)
        {
            seePlayer = true;
        }
        else
        {
            seePlayer = false;
        }
    }

    void MakeAIActions()
    {
        shootingTimer -= Time.deltaTime;
        _animations.AddIdle();
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
            _animations.SetAttack();
        }
    }

    void MoveTowardsPlayer()
    {
        Vector3 toPlayer = player.transform.position - this.transform.position;
        this.GetComponent<Rigidbody2D>().velocity = toPlayer.normalized * speed;
    }

    void ShootAtPlayer()
    {
        switch (aiMode)
        {
            case AIType.SHOOTER:
                {
                    GameObject newBullet = objectPoolManager.GetFreeObject("AIProjectileProto");
                    Vector3 toPlayer = player.transform.position - this.transform.position;
                    AIProjectileScript projScript = newBullet.GetComponent<AIProjectileScript>();
                    projScript.SetBulletDirectionAndSpeed(toPlayer, 8);
                    newBullet.transform.position = this.transform.position + toPlayer.normalized;
                    projScript.owner = this.gameObject;
                    projScript.toBeDestroyed = false;
                    projScript.projectileTimer = 5.0f;
                    break;
                }
            case AIType.BLASTER:
                {
                    for (int i = 0; i < 5; i++)
                    {
                        GameObject newBullet = objectPoolManager.GetFreeObject("AIProjectileProto");
                        Vector3 toPlayer = player.transform.position - this.transform.position;
                        AIProjectileScript projScript = newBullet.GetComponent<AIProjectileScript>();
                        float randomAngle = Random.Range(-20.0f, 20.0f);
                        Vector3 bulletDir = Quaternion.Euler(0, 0, randomAngle) * toPlayer;
                        projScript.SetBulletDirectionAndSpeed(bulletDir, 8);
                        newBullet.transform.position = this.transform.position + toPlayer.normalized;
                        projScript.owner = this.gameObject;
                        projScript.toBeDestroyed = false;
                        projScript.projectileTimer = 5.0f;
                    }
                    break;
                }
            case AIType.SPRAYER:
                {

                    StartCoroutine(SprayerShooting());
                    break;
                }
        }
    }

    IEnumerator SprayerShooting()
    {
        for (int i = -2; i < 3; i++)
        {
            yield return new WaitForSeconds(0.1f);
            GameObject newBullet = objectPoolManager.GetFreeObject("AIProjectileProto");
            Vector3 toPlayer = player.transform.position - this.transform.position;
            AIProjectileScript projScript = newBullet.GetComponent<AIProjectileScript>();
            float bulletMaxAngle = 20.0f;
            Vector3 bulletDir = Quaternion.Euler(0, 0, bulletMaxAngle * i) * toPlayer;
            projScript.SetBulletDirectionAndSpeed(bulletDir, 8);
            newBullet.transform.position = this.transform.position + toPlayer.normalized;
            projScript.owner = this.gameObject;
            projScript.toBeDestroyed = false;
            projScript.projectileTimer = 5.0f;
        }
    }

    public void TakeDamage(float damage, Vector2 damageDir)
    {
        _animations.SetHurt();
        health -= damage;
        hitStunTimer = 0.0f;
        //inStun = true;
        this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        
        if (takesKnockback && health > 0)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = -damageDir.normalized * 4.0f;
        }
    }

    void OnDeath()
    {
        if (deadTimer == 0.0f)
        {
            _animations.SetDeath();
        }

        deadTimer += Time.deltaTime;

        if (deadTimer > 2.0f)
        {
            GetComponent<enemyScr>().releaseEnemy();
        }
    }
}
