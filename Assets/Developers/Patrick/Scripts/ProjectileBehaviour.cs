using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    private ObjectPoolManager poolManager;
    private string objectName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InitialiseComponent(Vector2 moveDirection, float speed, ref ObjectPoolManager dPoolManager, string prefabName)
    {
        GetComponent<Rigidbody2D>().velocity = moveDirection * speed;
        poolManager = dPoolManager;

        objectName = prefabName;
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
            else if (collision.gameObject.GetComponent<AISimpleDetectionScript>() != null)
            {
                collision.gameObject.GetComponent<AISimpleDetectionScript>().TakeDamage(1);
            }

            poolManager.ReleaseObject(objectName, gameObject);
        }
        else if (collision.gameObject.layer == 6)
        {
            poolManager.ReleaseObject(objectName, gameObject);
        }
    }
}
