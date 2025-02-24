using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoguePopup : MonoBehaviour
{
    CanvasGroup canvasGroup;
    [SerializeField]
    private string text;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("collided with player");
            collision.gameObject.GetComponent<PlayerManager>().StartFadeInSpeech(text);

        }
    }
}
