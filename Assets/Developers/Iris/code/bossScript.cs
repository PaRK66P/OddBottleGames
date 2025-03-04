using UnityEngine;

public class bossScript : MonoBehaviour
{
    public int health = 100;
    public float restPeriod = 3;
    float restTimer = 0;
    bool pauseRest = false;
    int attackNo = 0;

    [SerializeField]
    private Transform TopLeft;
    [SerializeField] private Transform BottomRight;

    [Space]
    [Header("Attacks variables")]
    public GameObject artileryPrefab;
    [Header("Random artilery strike")]
    public int randomArtileryProjectileNo = 10;
    [Header("Targeted artilery strike")]
    public int targetedArtileryProjectileNo = 5;
    public float timeBetweenStrikes = 0.5f;
    int strikesSpawned = 0;
    float strikesTimer;
    [Header("Projectiles attack")]
    public GameObject projectilePrefab;
    public int projectileAttackNo = 10;

    public void takeDamage(int dmg)
    {
        health -= dmg;

        if(health <= 0)
        {
            die();
        }
    }

    void die()
    {
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!pauseRest)
        {
            restTimer += Time.deltaTime;
            if (restTimer >= restPeriod)
            {
                //attackNo = 1;
                attackNo = UnityEngine.Random.Range(0, 3);

                strikesSpawned = 0;
                strikesTimer = timeBetweenStrikes;

                //Debug.Log("strikes reset");

                Attack();
                restTimer = 0;
            }
        }
        else
        {
            Attack();
        }
    }

    void Attack()
    {
        switch (attackNo)
        {
            case 0:
                {
                    RandomArtileryAttack();
                    break;
                }
            case 1:
                {
                    pauseRest = true;
                    TargetedArtileryStrike();
                    break;
                }
            case 2:
                {
                    CircularProjectiles();
                    break;
                }
            default:
                break;
        }
    }

    void RandomArtileryAttack()
    {
        for (int i = 0; i<= randomArtileryProjectileNo; ++i)
        {
            //UnityEngine.Vector3 pos = new UnityEngine.Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), 0);
            UnityEngine.Vector3 pos = new UnityEngine.Vector3(UnityEngine.Random.Range(TopLeft.position.x, BottomRight.position.x), UnityEngine.Random.Range(BottomRight.position.y, TopLeft.position.y), 0);

            Instantiate(artileryPrefab, pos, UnityEngine.Quaternion.Euler(0, 0, 0));
        }
    }

    void TargetedArtileryStrike()
    {
        //Debug.Log("Strike");
        strikesTimer += Time.deltaTime;
        if(strikesTimer >= timeBetweenStrikes)
        {
            strikesTimer = 0;
            if (strikesSpawned < 5)
            {
                UnityEngine.Vector3 pos = new UnityEngine.Vector3(0, 0, 0);

                if (GameObject.FindGameObjectWithTag("Player"))
                {
                    pos = GameObject.FindGameObjectWithTag("Player").transform.position;
                }

                Instantiate(artileryPrefab, pos, UnityEngine.Quaternion.Euler(0, 0, 0));

                //Debug.Log(strikesSpawned);
                strikesSpawned++;
            }
            else
            {
                pauseRest = false;
                //Debug.Log(pauseRest);
            }
        }
    }

    void CircularProjectiles()
    {
        float rotStep = 360 / projectileAttackNo;
        UnityEngine.Vector3 rotation = new UnityEngine.Vector3(0, 0, 0);

        for (int i = 0; i <projectileAttackNo; i++)
        {
            Instantiate(projectilePrefab, gameObject.transform.position, UnityEngine.Quaternion.Euler(rotation));
            rotation.z += rotStep;
        }
    }
}
