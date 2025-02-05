using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Instantiate(Vector2 moveDirection, float speed)
    {
        GetComponent<Rigidbody2D>().velocity = moveDirection * speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 9)
        {
            Debug.Log("Trigger");

            if(collision.gameObject.tag == "Companion")
            {
                collision.gameObject.GetComponentInParent<CompanionLogic>().TakeDamage(1);
            }
            else if(collision.gameObject.tag == "Boss")
            {
                collision.gameObject.GetComponent<bossScript>().takeDamage(1);
            }
            else if (collision.gameObject.GetComponent<AISimpleBehaviour>() != null)
            {
                collision.gameObject.GetComponent<AISimpleBehaviour>().TakeDamage(1);
            }

            Debug.Log(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
