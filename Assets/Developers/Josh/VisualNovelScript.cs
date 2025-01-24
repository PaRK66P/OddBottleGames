using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class VisualNovelScene
{
    public VisualNovelScene(Sprite sprite, string newText)
    {
        CharacterAsset = sprite;
        text = newText;
    }
    public string text;
    public Sprite CharacterAsset;
}

public class VisualNovelScript : MonoBehaviour
{
    

    //[SerializeField]
    public List<VNPrefabScript> VNScenes = new List<VNPrefabScript>();

    public bool isNovelSection;
    public string newtext;
    public GameObject canv;
    public GameObject text;
    public GameObject sprite;

    int currentSceneIndex = 0;
    int currentVNPrefabIndex = 0;

    void Start()
    {
        canv = GameObject.Find("VisualNovelCanvas");
        text = GameObject.Find("VisualNovelText");
        sprite = GameObject.Find("VisualNovelSprite");

        StartNovelScene(0);
    }
    void Update()
    {
        if (isNovelSection)
        {
            canv.SetActive(true);
        }
        else
        {
            canv.SetActive(false);
        }
        //text.GetComponent<TMP_Text>().text = newtext;
    }

    void StartNovelScene(int NovelSceneID)
    {
        currentVNPrefabIndex = NovelSceneID;
        currentSceneIndex = 0;
        isNovelSection = true;
        if (currentVNPrefabIndex < VNScenes.Count && currentVNPrefabIndex > -1)
        {
            if (currentSceneIndex < VNScenes[currentVNPrefabIndex].Scenes.Count && currentSceneIndex > -1)
            {
                sprite.GetComponent<Image>().sprite = VNScenes[currentVNPrefabIndex].Scenes[currentSceneIndex].CharacterAsset;
                text.GetComponent<TMP_Text>().text = VNScenes[currentVNPrefabIndex].Scenes[currentSceneIndex].text;
            }
            else
            {
                Debug.LogError("currentSceneIndex out of range: " + VNScenes[currentVNPrefabIndex].Scenes.Count);
            }
        }
        else 
        {
            isNovelSection = false;
            UnityEngine.Debug.LogError("Invalid Novel Scene ID");
        }
    }
    void NextScene ()
    {
        currentSceneIndex += 1;
        if (currentSceneIndex < VNScenes.Count)
        {
            sprite.GetComponent<Image>().sprite = VNScenes[currentVNPrefabIndex].Scenes[currentSceneIndex].CharacterAsset;
            text.GetComponent<TMP_Text>().text = VNScenes[currentVNPrefabIndex].Scenes[currentSceneIndex].text;
        }
        else 
        {
            isNovelSection = false;
        }
    }
}
