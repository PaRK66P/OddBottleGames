using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLogic : MonoBehaviour
{
    [SerializeField] GameObject menuAssets;
    [SerializeField] GameObject loadingScreenAssets;
    [SerializeField] GameObject SettingsAssets;

    [SerializeField] UnityEngine.UI.Image loadingBar;
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

    public void OnMenuClick()
    {
        SettingsAssets.SetActive(false);
        menuAssets.SetActive(true);
    }
}
