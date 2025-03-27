using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class largeProjectileScript : MonoBehaviour
{
    public Rigidbody2D rb;
    
    private ObjectPoolManager pooler;
    private string prefabName;

    private float travelTimer;
    private float lifetime;

    private void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            pooler.ReleaseObject(prefabName, gameObject);
        }

        travelTimer -= Time.deltaTime;
        if (travelTimer <= 0)
        {
            rb.velocity = new Vector2(0, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        

        if (collision.gameObject.tag == "Player")
        {
            Vector2 damageDirection = new Vector2(collision.gameObject.transform.position.x - transform.position.x,
                    collision.gameObject.transform.position.y - transform.position.y);
            collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damageDirection.normalized, 1, 15);
        }
    }

    public void InstantiateComponent(ref ObjectPoolManager poolMan, string prefName, Vector3 pos, Vector3 rot, float speed, float dTravelTimer, float dLifetime)
    {
        pooler = poolMan;
        prefabName = prefName;
        transform.position = pos;
        transform.rotation = UnityEngine.Quaternion.Euler(rot);
        rb.velocity = transform.up * speed * -1;
        travelTimer = dTravelTimer;
        lifetime = dLifetime;
    }
}
