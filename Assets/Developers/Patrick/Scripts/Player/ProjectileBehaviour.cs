using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public Vector3 originalScale = new Vector3(1, 1, 1);
    public int originalDamage = 1;

    private ObjectPoolManager poolManager;
    private string objectName;
    private GameObject playerRef;
    private bool toBeReleased = false;
    private float lifeSpan = 0.0f;
    private float minLife = 0.05f;
    private int damage = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InitialiseComponent(Vector2 moveDirection, float speed, ref ObjectPoolManager dPoolManager, string prefabName, GameObject playerRefrence, float damageMultiplier)
    {
        GetComponent<Rigidbody2D>().velocity = moveDirection * speed;
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
        lifeSpan += Time.deltaTime;
        if (toBeReleased && lifeSpan > minLife)
        {
            poolManager.ReleaseObject("ProjectileProto", this.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (playerRef != null)
        {

            playerRef.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 9)
        {
            //Debug.Log("Trigger");

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

            toBeReleased = true;
        }
        else if (collision.gameObject.layer == 6)
        {
            // Vector2 projDir = GetComponent<Rigidbody2D>().velocity.normalized;
            // Vector2 dirToCollisionObject = collision.gameObject.transform.position - gameObject.transform.position;

            // float angle = Vector2.Angle(projDir, dirToCollisionObject);
            // Debug.Log(angle);


            // //Debug.Log(angle);
            // if (angle < 125.0f)
            // {
            //     toBeReleased = true;
            // }
            toBeReleased = true;
        }
    }
}
