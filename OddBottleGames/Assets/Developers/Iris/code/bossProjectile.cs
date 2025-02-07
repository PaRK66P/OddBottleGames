using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossProjectile : MonoBehaviour
{
    public float speed;
    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.right * speed;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Vector2 damageDirection = new Vector2(collision.gameObject.transform.position.x - transform.position.x,
                    collision.gameObject.transform.position.y - transform.position.y);
            collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damageDirection.normalized);
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
