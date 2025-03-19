using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public Vector3 originalScale = new Vector3(1, 1, 1);
    public int originalDamage = 1;
    public Vector2 _moveDirection;

    private ObjectPoolManager poolManager;
    private string objectName;
    private GameObject playerRef;
    private bool toBeReleased = false;
    private float lifeSpan = 0.0f;
    private float minLife = 0.05f;
    private int damage = 1;

    private bool _hasCompanion = false;
    private CompanionManager _companionManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InitialiseComponent(Vector2 moveDirection, float speed, ref ObjectPoolManager dPoolManager, string prefabName, GameObject playerRefrence, float damageMultiplier)
    {
        GetComponent<Rigidbody2D>().velocity = moveDirection * speed;
        _moveDirection = moveDirection;
        poolManager = dPoolManager;

        objectName = prefabName;
        playerRef = playerRefrence;
        lifeSpan = 0.0f;
        toBeReleased = false;

        transform.localScale = originalScale;
        damage = originalDamage;
        if (damageMultiplier != 1)
        {
            transform.localScale = originalScale * (1 + (damageMultiplier * 0.2f));
            damage = (int)(originalDamage * damageMultiplier);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (toBeReleased)
        {
            if(lifeSpan > minLife)
            {
                poolManager.ReleaseObject(objectName, this.gameObject);
            }
            lifeSpan += Time.deltaTime;
        }
    }

    private void OnDestroy()
    {
        if (playerRef != null)
        {
            playerRef.SetActive(false);
        }
    }

    public void SetToRelease()
    {
        toBeReleased = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 9)
        {

            if(collision.gameObject.tag == "Companion")
            {
                collision.gameObject.GetComponentInParent<CompanionManager>().TakeDamage(damage);
            }
            else if(collision.gameObject.tag == "Boss")
            {
                collision.gameObject.GetComponent<boss>().takeDamage(damage);
            }
            else if (collision.gameObject.GetComponent<AISimpleBehaviour>() != null)
            {
                collision.gameObject.GetComponent<AISimpleBehaviour>().TakeDamage(damage, gameObject.transform.position - collision.gameObject.transform.position);
            }

            if (_hasCompanion)
            {
                if (collision.gameObject.GetComponent<CompanionTargettingHandler>() != null)
                {
                    collision.gameObject.GetComponent<CompanionTargettingHandler>().InitialiseTargetting(ref _companionManager);
                }
            }

            lifeSpan = minLife;
            toBeReleased = true;
        }
    }

    public void AddCompanionTargetting(ref CompanionManager companionManager)
    {
        _hasCompanion = true;
        _companionManager = companionManager;
    }
}
