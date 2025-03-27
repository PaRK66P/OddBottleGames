using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoguePopup : MonoBehaviour
{
    CanvasGroup canvasGroup;
    [SerializeField]
    private string text;
    private bool hasBeenActivated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasBeenActivated)
        {
            if (collision.gameObject.tag == "Player")
            {
                collision.gameObject.GetComponent<PlayerManager>().StartFadeInSpeech(text);
                hasBeenActivated = true;
            }
            
        }
    }
}
