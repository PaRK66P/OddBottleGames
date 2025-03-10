using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public bool keyCollected = false;
    private bool isPlayerInRange = false;
    public GameObject player;

    public void Update()
    {
        if (isPlayerInRange && keyCollected)
        {
            if (player.GetComponent<PlayerManager>().isInteracting())
            {
                this.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isPlayerInRange = true;
            player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isPlayerInRange = false;
            
        }
    }
}
