using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempDamage : MonoBehaviour
{
    float health = 2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            Vector2 damageDirection = new Vector2(collision.gameObject.transform.position.x - transform.position.x,
                collision.gameObject.transform.position.y - transform.position.y);
            collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damageDirection.normalized);
        }
    }

    public void TakeDamage(float hurtValue)
    {
        health -= hurtValue;

        if(health < 0)
        {
            Destroy(gameObject);
        }
    }
}
