using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    private GameObject player;
    private GameObject pauseMenu;
    private bool isPaused = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerProto");
        pauseMenu = GameObject.Find("Canvas").transform.Find("PauseMenu").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                OnPause();
            }
            else
            {
                OnUnpause();
            }
        }
    }

    private void OnPause()
    {
        Time.timeScale = 0.0f;
        player.GetComponent<PlayerManager>().DisableInput();

        pauseMenu.SetActive(true);
        isPaused = true;

    }

    private void OnUnpause()
    {
        Time.timeScale = 1.0f;
        player.GetComponent<PlayerManager>().EnableInput();

        pauseMenu.SetActive(false);
        isPaused = false;

    }
}
