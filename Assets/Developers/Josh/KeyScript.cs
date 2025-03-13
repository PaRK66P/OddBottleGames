using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour
{
    [SerializeField] private DoorScript door;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //door.gameObject.SetActive(false);
            door.keyCollected = true;
            this.gameObject.SetActive(false);
        }
    }
}
