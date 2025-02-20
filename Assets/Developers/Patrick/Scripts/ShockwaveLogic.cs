using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ShockwaveLogic : MonoBehaviour
{
    private float speed;
    private Vector2 directionMovement;
    private float damage;
    private LayerMask target;

    private Rigidbody2D rb;

    private ObjectPoolManager objectPoolManager;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + directionMovement * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer) == target.value)
        {
            if (collision.gameObject.GetComponent<PlayerManager>() != null)
            {
                Vector2 damageDirection = new Vector2(collision.gameObject.transform.position.x - transform.position.x,
                    collision.gameObject.transform.position.y - transform.position.y);
                collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damageDirection.normalized, 1, 10);
            }
            else if (collision.gameObject.GetComponent<AISimpleBehaviour>() != null)
            {
                collision.gameObject.GetComponent<AISimpleBehaviour>().TakeDamage(damage);
            }
            else if (collision.gameObject.GetComponent<bossScript>() != null)
            {
                collision.gameObject.GetComponent<bossScript>().takeDamage(1);
            }
        }
        if(collision.gameObject.layer == 6)
        {
            objectPoolManager.ReleaseObject("Shockwave", this.gameObject);
        }
    }

    public void InitialiseEffect(LayerMask damageLayer, float totalDamage, Vector2 direction, float speedMovement, ObjectPoolManager objMgr)
    {
        target = damageLayer;
        damage = totalDamage;
        directionMovement = direction;
        speed = speedMovement;
        objectPoolManager = objMgr;
    }


}
