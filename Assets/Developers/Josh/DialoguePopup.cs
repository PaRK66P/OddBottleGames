using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoguePopup : MonoBehaviour
{
    GameObject canvas;
    private void Start()
    {
        canvas = GameObject.Find("Canvas");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
