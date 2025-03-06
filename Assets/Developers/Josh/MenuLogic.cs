using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLogic : MonoBehaviour
{
    [SerializeField] GameObject MenuAssets;

    [SerializeField] GameObject LoadingScreenAssets;

    [SerializeField] GameObject LoadingBar;
    // Start is called before the first frame update
    void OnStartClick()
    {
        StartCoroutine(LoadGame());
        MenuAssets.SetActive(false);
        LoadingScreenAssets.SetActive(true);
    }

    IEnumerator LoadGame()
    {
        AsyncOperation loadGame = SceneManager.LoadSceneAsync("PrototypeScene");

        while (!loadGame.isDone)
        {
            float fillAmount = Mathf.Clamp01(loadGame.progress / 0.9f);
            yield return null;
        }
    }
}
