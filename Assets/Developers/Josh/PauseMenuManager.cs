
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Pause menu assets")]
    [SerializeField] UnityEngine.UI.Slider volumeSlider;
    [SerializeField] UnityEngine.UI.Slider typingSpeedSlider;
    [SerializeField] UnityEngine.UI.Toggle dashToggle;
    [SerializeField] UnityEngine.UI.Toggle autoTypeToggle;

    [Header("Important references")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject pauseMenuContainer;
    [SerializeField] private VisualNovelScript visualNovelManager;
    private bool isPaused = false;

    private float volume = 0.3f;
    private float typingSpeed = 1.0f;
    private bool dash = false;
    private bool autoType = true;

    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.Find("Player");
        //pauseMenuContainer = GameObject.Find("Canvas").transform.Find("PauseMenu").gameObject;
        //visualNovelManager = GameObject.Find("VisualNovelManager").GetComponent<VisualNovelScript>();


        if (!PlayerPrefs.HasKey("volume"))
            PlayerPrefs.SetFloat("volume", volume);
        volume = PlayerPrefs.GetFloat("volume", volume);
        volumeSlider.value = volume;

        if (!PlayerPrefs.HasKey("typingSpeed"))
            PlayerPrefs.SetFloat("typingSpeed", typingSpeed);
        typingSpeed = PlayerPrefs.GetFloat("typingSpeed", typingSpeed);
        typingSpeedSlider.value = typingSpeed;

        if (!PlayerPrefs.HasKey("dash"))
            PlayerPrefs.SetInt("dash", BoolToInt(dash));
        dash = IntToBool(PlayerPrefs.GetInt("dash", BoolToInt(dash)));
        dashToggle.isOn = dash;

        if (!PlayerPrefs.HasKey("autoType"))
            PlayerPrefs.SetInt("autoType", BoolToInt(autoType));
        autoType = IntToBool(PlayerPrefs.GetInt("autoType", BoolToInt(autoType)));
        autoTypeToggle.isOn = autoType;

        PlayerPrefs.Save();
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
        pauseMenuContainer.transform.Find("SettingsScreen").gameObject.SetActive(false);
        isPaused = true;
    }

    private void OnUnpause()
    {
        PlayerPrefs.Save();
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
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OnControlsClick()
    {
        pauseMenuContainer.transform.Find("MainPause").gameObject.SetActive(false);
        pauseMenuContainer.transform.Find("SettingsScreen").gameObject.SetActive(true);
    }

    public void OnControlsBackClick()
    {
        pauseMenuContainer.transform.Find("MainPause").gameObject.SetActive(true);
        pauseMenuContainer.transform.Find("SettingsScreen").gameObject.SetActive(false);
    }

    public void OnRestartClick()
    {
        //pauseMenuContainer.SetActive(false);
        OnResumeClick();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void OnVolumeSliderChanged(float value)
    {
        volume = value;
        PlayerPrefs.SetFloat("volume", volume);
    }

    public void OnTextSpeedSliderChange(float value)
    {
        typingSpeed = value;
        PlayerPrefs.SetFloat("typingSpeed", typingSpeed);
    }

    public void OnTypingTextToggleChanged(bool toggle)
    {
        autoType = toggle;
        PlayerPrefs.SetInt("autoType", BoolToInt(autoType));
    }

    public void DashToMouseToggleChanged(bool toggle)
    {
        dash = toggle;
        PlayerPrefs.SetInt("dash", BoolToInt(dash));
    }

    private int BoolToInt(bool val)
    {
        if (val == true)
            return 1;
        else
            return 0;
    }

    private bool IntToBool(int val)
    {
        if (val == 0)
            return false;
        else
            return true;
    }
}
