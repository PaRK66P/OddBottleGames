using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    private ObjectPoolManager poolManager;
    private string objectName;
    private GameObject playerRef;

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
    }

    // Update is called once per frame
    void Update()
    {
        
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

            poolManager.ReleaseObject(objectName, gameObject);
        }
        else if (collision.gameObject.layer == 6)
        {
            poolManager.ReleaseObject(objectName, gameObject);
        }
    }
}
