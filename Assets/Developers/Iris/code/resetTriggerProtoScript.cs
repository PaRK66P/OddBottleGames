using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class resetTriggerProtoScript : MonoBehaviour
{
    public GameObject endScreen;
    public GameObject player;

    bool started = false;
    bool ended = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.FindGameObjectWithTag("Boss"))
        {
            if(!started)
            {
                started = true;
            }
        }
        else
        {
            if(started)
            {
                ended = true;
                GetComponent<SpriteRenderer>().enabled = true;
                GetComponent<BoxCollider2D>().enabled = true;
                GetComponentInChildren<TextMeshPro>().enabled = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(ended)
        {
            if (collision.gameObject.tag == "Player")
            {
                //SceneManager.LoadScene("PrototypeScene");
                endScreen.SetActive(true);
                player.GetComponent<PlayerInputManager>().DisableInput();
            }
        }
    }
}
