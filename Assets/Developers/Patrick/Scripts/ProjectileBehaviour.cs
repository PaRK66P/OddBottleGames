using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    private ObjectPoolManager poolManager;
    private string objectName;
    private GameObject playerRef;
    private bool toBeReleased = false;
    private float lifeSpan = 0.0f;
    private float minLife = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InitialiseComponent(Vector2 moveDirection, float speed, ref ObjectPoolManager dPoolManager, string prefabName, GameObject playerRefrence)
    {
        GetComponent<Rigidbody2D>().velocity = moveDirection * speed;
        poolManager = dPoolManager;

        objectName = prefabName;
        playerRef = playerRefrence;
        lifeSpan = 0.0f;
        toBeReleased = false;
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
                collision.gameObject.GetComponentInParent<CompanionLogic>().TakeDamage(1);
            }
            else if(collision.gameObject.tag == "Boss")
            {
                collision.gameObject.GetComponent<boss>().takeDamage(1);
            }
            else if (collision.gameObject.GetComponent<AISimpleBehaviour>() != null)
            {
                collision.gameObject.GetComponent<AISimpleBehaviour>().TakeDamage(1);
            }

            toBeReleased = true;
        }
        else if (collision.gameObject.layer == 6)
        {
            Vector2 projDir = GetComponent<Rigidbody2D>().velocity.normalized;
            Vector2 dirToCollisionObject = collision.gameObject.transform.position - gameObject.transform.position;

            float angle = Vector2.Angle(projDir, dirToCollisionObject);
            
            //Debug.Log(angle);
            if (angle < 90.0f)
            {
                toBeReleased = true;
            }
        }
    }
}
