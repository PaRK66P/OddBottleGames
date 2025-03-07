using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLogic : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] GameObject menuAssets;

    [Header("Settings")]
    [SerializeField] GameObject SettingsAssets;
    [SerializeField] Slider volumeSlider;

    [Header("Loading")]
    [SerializeField] GameObject loadingScreenAssets;
    [SerializeField] UnityEngine.UI.Image loadingBar;

    private float volume = 0.0f;
    private float typingSpeed = 1.0f;
    private bool dashToggle = false;
    private bool autoType = false;

    // Start is called before the first frame update
    public void OnStartClick()
    {
        menuAssets.SetActive(false);
        loadingScreenAssets.SetActive(true);
        StartCoroutine(LoadGame());
        
    }

    IEnumerator LoadGame()
    {
        AsyncOperation loadGame = SceneManager.LoadSceneAsync("PrototypeScene");

        while (!loadGame.isDone)
        {
            float fillAmount = Mathf.Clamp01(loadGame.progress / 0.9f);
            loadingBar.fillAmount = fillAmount;
            yield return null;
        }
    }

    public void OnSettingsClick()
    {
        menuAssets.SetActive(false);
        SettingsAssets.SetActive(true);
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }

    public void OnMenuClick()
    {
        SettingsAssets.SetActive(false);
        menuAssets.SetActive(true);
    }

    public void OnVolumeSliderChanged(float value)
    {
        volume = value;
    }

    public void OnTextSpeedSliderChange(float value)
    {
        typingSpeed = value;
    }

    public void OnTypingTextToggleChanged(bool toggle)
    {
        autoType = toggle;
    }

    public void DashToMouseToggleChanged(bool toggle)
    {
        dashToggle = toggle;
    }

}
