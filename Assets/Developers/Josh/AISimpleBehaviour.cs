using System.Collections;
using System.Collections.Generic;
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
    private List<GameObject> particles = new List<GameObject>();

    public GameObject AIProjectilePrefab;
    [SerializeField]
    private List<GameObject> projectiles = new List<GameObject>();

    public ObjectPoolManager objectPoolManager;

    public AIType aiMode = AIType.SHOOTER;

    private Color currentColor;


    public void Instantiate(ref ObjectPoolManager bObjectPoolManager, ref GameObject bPlayer)
    {
        isDead = false;
        shootingTimer = 0.0f;
        deadTimer = 0.0f;
        health = maxHealth;
        inStun = false;
        objectPoolManager = bObjectPoolManager;
        player = bPlayer;
        currentColor = GetComponent<SpriteRenderer>().color;
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
                BulletCleanUp();

            }
        }
        if (health <= 0)
        {
            GetComponent<CompanionTargettingHandler>().ReleaseAsTarget();
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
             float angle = Mathf.Atan2(distToPlayer.y, distToPlayer.x) * Mathf.Rad2Deg;
             float targetRotation = Mathf.LerpAngle(this.transform.rotation.eulerAngles.z, angle, Time.deltaTime * 10);
             this.transform.rotation = Quaternion.Euler(0, 0, targetRotation);

        }
        else
        {
            seePlayer = false;
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
                    projectiles.Add(newBullet);
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
                        projectiles.Add(newBullet);
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
            projectiles.Add(newBullet);
        }
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

    public void TakeDamage(float damage, Vector2 damageDir)
    {
        health -= damage;
        StartCoroutine(DamageColor());
        hitStunTimer = 0.0f;
        //inStun = true;
        this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        
        if (takesKnockback && health > 0)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = -damageDir.normalized * 4.0f;
        }
    }

    IEnumerator DamageColor()
    {
        SpriteRenderer spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = currentColor;
    }

    void OnDeath()
    {
        if (deadTimer == 0.0f)
        {
            
            for (int i = 0; i < 8; i++)
            {
                particles.Add(Instantiate(particle));
                float speed = 3.0f;
                float angle = Mathf.PI * 2.0f * i/8.0f;
                float x = speed * Mathf.Cos(angle);
                float y = speed * Mathf.Sin(angle);
                particles[i].GetComponent<Rigidbody2D>().velocity = new Vector2(x, y);
                particles[i].transform.rotation = Quaternion.Euler(0,0,Mathf.Rad2Deg*angle);
                particles[i].transform.position = gameObject.transform.position + (new Vector3(x,y,0)*0.4f);
            }
        }

        deadTimer += Time.deltaTime;

        foreach (GameObject particle in particles)
        {
            particle.GetComponent<Rigidbody2D>().velocity *= 0.95f;
        }

        if (deadTimer > 0.3f)
        {
            foreach (GameObject particle in particles)
            {
                Destroy(particle);
            }
            particles.Clear();
            GetComponent<enemyScr>().releaseEnemy();
        }
    }
}
