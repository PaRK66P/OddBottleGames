using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLogic : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] GameObject menuAssets;

    //[Header("Settings")]
    //[SerializeField] GameObject SettingsAssets;
    //[SerializeField] Slider volumeSlider;
    //[SerializeField] Slider typingSpeedSlider;
    //[SerializeField] Toggle dashToggle;
    //[SerializeField] Toggle autoTypeToggle;

    //[Header("Loading")]
    //[SerializeField] GameObject loadingScreenAssets;
    //[SerializeField] UnityEngine.UI.Image loadingBar;

    private float volume = 0.3f;
    private float typingSpeed = 1.0f;
    private bool dash = false;
    private bool autoType = true;

    // Start is called before the first frame update
    public void Start()
    {
        //PlayerPrefs.DeleteAll();
        if (!PlayerPrefs.HasKey("volume"))
           PlayerPrefs.SetFloat("volume", volume);
        //volume = PlayerPrefs.GetFloat("volume", volume);
        //volumeSlider.value = volume;

        if (!PlayerPrefs.HasKey("typingSpeed"))
           PlayerPrefs.SetFloat("typingSpeed", typingSpeed);
        //typingSpeed = PlayerPrefs.GetFloat("typingSpeed", typingSpeed);
        //typingSpeedSlider.value = typingSpeed;
        
        if (!PlayerPrefs.HasKey("dash"))
           PlayerPrefs.SetInt("dash", BoolToInt(dash));
        //dash = IntToBool(PlayerPrefs.GetInt("dash", BoolToInt(dash)));
        //dashToggle.isOn = dash;
        
        if (!PlayerPrefs.HasKey("autoType"))
           PlayerPrefs.SetInt("autoType", BoolToInt(autoType));
        //autoType = IntToBool(PlayerPrefs.GetInt("autoType", BoolToInt(autoType)));
        //autoTypeToggle.isOn = autoType;

        //PlayerPrefs.Save();

    }

    //private int BoolToInt(bool val)
    //{
    //    if (val == true)
    //        return 1;
    //    else
    //        return 0;
    //}

    //private bool IntToBool(int val)
    //{
    //    if (val == 0)
    //        return false;
    //    else
    //        return true;
    //}
    public void OnStartClick()
    {
        menuAssets.SetActive(false);
        //loadingScreenAssets.SetActive(true);
        StartCoroutine(LoadGame());
        
    }

    IEnumerator LoadGame()
    {
        PlayerPrefs.Save();
        AsyncOperation loadGame = SceneManager.LoadSceneAsync("MAP 8");

        yield return null;

        //while (!loadGame.isDone)
        //{
        //    float fillAmount = Mathf.Clamp01(loadGame.progress / 0.9f);
        //    loadingBar.fillAmount = fillAmount;
        //    yield return null;
        //}
    }

    //public void OnSettingsClick()
    //{
    //    menuAssets.SetActive(false);
    //    SettingsAssets.SetActive(true);
    //}

    public void OnQuitClick()
    {
        Application.Quit();
    }

    //public void OnMenuClick()
    //{
    //    PlayerPrefs.Save();
    //    SettingsAssets.SetActive(false);
    //    menuAssets.SetActive(true);
    //}

    //public void OnVolumeSliderChanged(float value)
    //{
    //    volume = value;
    //    PlayerPrefs.SetFloat("volume", value);
    //}

    //public void OnTextSpeedSliderChange(float value)
    //{
    //    typingSpeed = value;
    //    PlayerPrefs.SetFloat("typingSpeed", value);
    //}

    //public void OnTypingTextToggleChanged(bool toggle)
    //{
    //    autoType = toggle;
    //    PlayerPrefs.SetInt("autoType", BoolToInt(toggle));
    //}

    //public void DashToMouseToggleChanged(bool toggle)
    //{
    //    dash = toggle;
    //    PlayerPrefs.SetInt("dash", BoolToInt(toggle));
    //}

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
