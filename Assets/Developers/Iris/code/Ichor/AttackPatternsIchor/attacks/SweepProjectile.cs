using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweepProjectile : MonoBehaviour
{
    
    public Rigidbody2D rb;

    private ObjectPoolManager pooler;
    private string prefabName;
    private float lifespan;
    private float chargeTimer;
    private Vector2 vel = new Vector2(0, 0);
    private bool wallCollision = false;
    private float speed;

    private void Update()
    {
        chargeTimer -= Time.deltaTime;
        if(chargeTimer <= 0)
        {
            rb.velocity = vel;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        lifespan -= Time.deltaTime;
        if (collision.gameObject.tag == "Player")
        {
            Vector2 damageDirection = new Vector2(collision.gameObject.transform.position.x - transform.position.x, collision.gameObject.transform.position.y - transform.position.y);
            collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damageDirection.normalized, 1, 15);
        }
        if (collision.gameObject.layer == 6 && wallCollision)
        {
            pooler.ReleaseObject(prefabName, gameObject);
        }
        else if(lifespan <= 0)
        {
            pooler.ReleaseObject(prefabName, gameObject);
        }
    }

    public void InstantiateComponent(ref ObjectPoolManager poolMan, string prefName, Vector3 pos, Vector3 rot, float dSpeed, float dChargeTimer, float dLifespan, bool dWallCollision)
    {
        pooler = poolMan;
        prefabName = prefName;
        transform.position = pos;
        transform.rotation = UnityEngine.Quaternion.Euler(rot);
        rb.velocity = new Vector2(0, 0);
        vel = transform.right * dSpeed;
        lifespan = dLifespan;
        chargeTimer = dChargeTimer;
        wallCollision = dWallCollision;
    }
}
