using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StartCutscene : MonoBehaviour
{
    bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("collision with trigger");
        if (!isTriggered)
        {
            Debug.Log("Triggered");
            if (collision.gameObject.tag == "Player")
            {
                GetComponent<PlayableDirector>().enabled = true;
                Debug.Log("playCutscene");
            }
        }
    }
}
