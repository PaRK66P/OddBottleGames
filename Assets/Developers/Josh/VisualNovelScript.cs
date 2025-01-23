using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class VisualNovelScene
{
   public string text;
    public Sprite CharacterAsset;
}

public class VisualNovelScript : MonoBehaviour
{
    

    //[SerializeField]
    public List<VisualNovelScene> VNScenes = new List<VisualNovelScene> {new VisualNovelScene()};

    public bool isNovelSection;
    public string newtext;
    public GameObject canv;
    public GameObject text;
    public GameObject sprite;

    int currentIndex = 0;

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
        text.GetComponent<TMP_Text>().text = newtext;
    }

    void StartNovelScene(int NovelSceneID)
    {
        currentIndex = NovelSceneID;
        isNovelSection = true;
        if (currentIndex < VNScenes.Count && currentIndex > -1)
        {
            sprite.GetComponent<Image>().sprite = VNScenes[currentIndex].CharacterAsset;
            text.GetComponent<TMP_Text>().text = VNScenes[currentIndex].text;
        }
        else 
        {
            isNovelSection = false;
            UnityEngine.Debug.LogError("Invalid Novel Scene ID");
        }
    }
    void NextScene ()
    {
        currentIndex += 1;
        if (currentIndex < VNScenes.Count)
        {
            sprite.GetComponent<Image>().sprite = VNScenes[currentIndex].CharacterAsset;
            text.GetComponent<TMP_Text>().text = VNScenes[currentIndex].text;
        }
        else 
        {
            isNovelSection = false;
        }
    }
}
