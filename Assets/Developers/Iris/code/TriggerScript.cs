using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    public enemyManager enemyMan;
    public bool isTriggered = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collision with trigger");
        if (!isTriggered)
        {
            if (collision.gameObject.tag == "Player")
            {
                enemyMan.lockDoors();
            }
        }        
    }
}
