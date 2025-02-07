using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class artileryAttack : MonoBehaviour
{
    public float delay = 1;
    public float activeTime = 1;

    float timeElapsed = 0;

    bool damage;

    // Start is called before the first frame update
    void Start()
    {
        damage = false;
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if(timeElapsed >= delay)
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
            gameObject.layer = LayerMask.NameToLayer("EnemyAttack");
            damage = true;
        }

        if(timeElapsed >= activeTime + delay)
        {
            Destroy(gameObject);
        }

        //collision with player function
            //player take damage
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && damage)
        {
            Vector2 damageDirection = new Vector2(collision.gameObject.transform.position.x - transform.position.x,
                    collision.gameObject.transform.position.y - transform.position.y);
            collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damageDirection.normalized);
        }
    }
}
