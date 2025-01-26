using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class VisualNovelScene
{
    public VisualNovelScene() { text = ""; entryText = ""; }
    public VisualNovelScene(Sprite sprite, string newText, string newEntryText)
    {
        CharacterAsset = sprite;
        text = newText;
        entryText = newEntryText;
    }
    public string entryText;
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

    DialogueTreeNode currentNode;
    int currentVNPrefabIndex = 0;


    void Start()
    {
        canv = GameObject.Find("VisualNovelCanvas");
        text = GameObject.Find("VisualNovelText");
        sprite = GameObject.Find("VisualNovelSprite");

        StartNovelSceneByName("test");
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
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextScene();
        }
    }

    void StartNovelScene(int NovelSceneID)
    {
        currentVNPrefabIndex = NovelSceneID;
        
        isNovelSection = true;
        if (currentVNPrefabIndex < VNScenes.Count && currentVNPrefabIndex > -1)
        {
            DialogueTree tree = new DialogueTree(ReconstructTree(VNScenes[currentVNPrefabIndex].tree));
            currentNode = tree.rootNode;
            text.GetComponent<TMP_Text>().text = currentNode.sceneData.text;
            sprite.GetComponent<Image>().sprite = currentNode.sceneData.CharacterAsset;
            
        }
        else 
        {
            isNovelSection = false;
            UnityEngine.Debug.LogError("Invalid Novel Scene ID");
        }
    }

    void StartNovelSceneByName(string name)
    {
        int index = 0;
        foreach (var scene in VNScenes)
        {
            if (scene.name == name)
            {
                StartNovelScene(index);
                return;
            }
            index++;
        }
        Debug.LogError("No scene found with name: " + name);
    }
    void NextScene ()
    {
        if (!currentNode.isLeaf())
        {
            currentNode = currentNode.children[0];
            sprite.GetComponent<Image>().sprite = currentNode.sceneData.CharacterAsset;
            text.GetComponent<TMP_Text>().text = currentNode.sceneData.text;
        }
        else 
        {
            isNovelSection = false;
        }
    }

    public DialogueTreeNode ReconstructTree(SerializedTree serializedTree)
    {
        //Debug.Log(serializedTree);
        var nodeDict = new Dictionary<int, DialogueTreeNode>();

        foreach (var serializedNode in serializedTree.nodes)
        {
            var node = new DialogueTreeNode(serializedNode.sceneData);
            nodeDict[serializedNode.id] = node;

            
        }
        foreach (var serializedNode in serializedTree.nodes)
        {
            if (serializedNode.parentId != 0)
            {
                var parentNode = nodeDict[serializedNode.parentId];
                parentNode.children.Add(nodeDict[serializedNode.id]);
            }
        }
        return nodeDict[serializedTree.nodes[0].id];
    }

    
}
