using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnablers : MonoBehaviour
{
    [SerializeField]
    GameObject[] enableObjects;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            foreach(GameObject obj in enableObjects)
            {
                obj.SetActive(true);
            }

            Destroy(gameObject);
        }
    }
}
