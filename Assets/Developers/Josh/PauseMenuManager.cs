
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    private GameObject player;
    private GameObject pauseMenuContainer;
    private VisualNovelScript visualNovelManager;
    private bool isPaused = false;
    private float volume;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerProto");
        pauseMenuContainer = GameObject.Find("Canvas").transform.Find("PauseMenu").gameObject;
        visualNovelManager = GameObject.Find("VisualNovelManager").GetComponent<VisualNovelScript>();
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

        pauseMenuContainer.SetActive(true);
        pauseMenuContainer.transform.Find("MainPause").gameObject.SetActive(true);
        pauseMenuContainer.transform.Find("ControlsScreen").gameObject.SetActive(false);
        isPaused = true;

    }

    private void OnUnpause()
    {
        Time.timeScale = 1.0f;
        player.GetComponent<PlayerManager>().EnableInput();

        pauseMenuContainer.SetActive(false);
        isPaused = false;

    }

    public void OnResumeClick()
    {
        OnUnpause();
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }

    public void OnControlsClick()
    {
        pauseMenuContainer.transform.Find("MainPause").gameObject.SetActive(false);
        pauseMenuContainer.transform.Find("ControlsScreen").gameObject.SetActive(true);
    }

    public void OnControlsBackClick()
    {
        pauseMenuContainer.transform.Find("MainPause").gameObject.SetActive(true);
        pauseMenuContainer.transform.Find("ControlsScreen").gameObject.SetActive(false);
    }

    public void OnRestartClick()
    {
        //pauseMenuContainer.SetActive(false);
        OnResumeClick();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void OnVolumeChange(float value)
    {
        volume = value;
    }

    public void OnTextSpeedSliderChange(float val)
    {
        visualNovelManager.typeTextSpeed = val;
    }

    public void OnTypingTextToggleChanged(bool toggle)
    {
        visualNovelManager.typingTextToggle = toggle;
    }

    public void DashToMouseToggleChanged(bool toggle)
    {
        player.GetComponent<PlayerMovement>().dashTowardsMouse = toggle;
    }
}
