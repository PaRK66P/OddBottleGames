using System.Collections.Generic;
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
    public int selectionID;
}

public class VisualNovelScript : MonoBehaviour
{
    //[SerializeField]
    List<VNPrefabScript> VNScenes = new List<VNPrefabScript>();

    public bool isNovelSection;
    public string newtext;
    public GameObject canv;
    public GameObject text;
    public GameObject sprite;

    public Transform buttonContainer;
    public GameObject buttonPrefab;
    private List<GameObject> buttons = new List<GameObject>();


    DialogueTreeNode currentNode;
    int currentVNPrefabIndex = 0;
    private int lastSelectionID = 0;


    void Start()
    {
        canv = GameObject.Find("VisualNovelCanvas");
        text = GameObject.Find("VisualNovelText");
        sprite = GameObject.Find("VisualNovelSprite");
        buttonContainer = GameObject.Find("VisualNovelButtonContainer").GetComponent<Transform>();

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Developers/Josh/VisualNovelScenes" });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab != null)
            {
                VNPrefabScript script = prefab.GetComponent<VNPrefabScript>();
                if (script != null)
                {
                    VNScenes.Add(prefab.GetComponent<VNPrefabScript>());
                }

            }
        }
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
    }

    public void StartNovelScene(int NovelSceneID)
    {
        currentVNPrefabIndex = NovelSceneID;

        isNovelSection = true;
        if (currentVNPrefabIndex < VNScenes.Count && currentVNPrefabIndex > -1)
        {
            DialogueTree tree = new DialogueTree(ReconstructTree(VNScenes[currentVNPrefabIndex].tree));
            currentNode = tree.rootNode;
            text.GetComponent<TMP_Text>().text = currentNode.sceneData.text;
            sprite.GetComponent<Image>().sprite = currentNode.sceneData.CharacterAsset;

            IDSelectionOptions(currentNode, 0);
            CreateButtons();

        }
        else
        {
            isNovelSection = false;
            UnityEngine.Debug.LogError("Invalid Novel Scene ID");
        }
    }

    public void StartNovelSceneByName(string name)
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
    void NextScene(int index)
    {
        if (!currentNode.isLeaf())
        {
            if (index > -1 && index < currentNode.children.Count)
            {
                currentNode = currentNode.children[index];
                sprite.GetComponent<Image>().sprite = currentNode.sceneData.CharacterAsset;
                text.GetComponent<TMP_Text>().text = currentNode.sceneData.text;
                CreateButtons();
            }
            else
            {
                Debug.LogError("tried to transition to invalid scene index");
            }
        }
        else
        {

            lastSelectionID = currentNode.sceneData.selectionID;
            Debug.Log("selectionID: " + lastSelectionID);
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

    public void CreateButtons()
    {
        foreach (GameObject button in buttons)
        {
            Destroy(button);
        }
        buttons.Clear();

        if (currentNode.isLeaf())
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
            newButton.name = "Button_" + 0;
            newButton.transform.localScale = new Vector3(2, 2, 1);

            Text buttonText = newButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = "continue";
            }

            Button buttonComponent = newButton.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.onClick.AddListener(() => NextScene(0));
            }

            buttons.Add(newButton);
        }
        for (int i = 0; i < currentNode.children.Count; i++)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
            newButton.name = "Button_" + i;
            newButton.transform.localPosition = new Vector3(0, -90 * i, 0);
            newButton.transform.localScale = new Vector3(2, 2, 1);


            Text buttonText = newButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = currentNode.children[i].sceneData.entryText;
            }

            int index = i;
            Button buttonComponent = newButton.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.onClick.AddListener(() => NextScene(index));
            }

            buttons.Add(newButton);
        }
    }

    int IDSelectionOptions(DialogueTreeNode node, int currentIDCount)
    {
        if (node.isLeaf())
        {
            node.sceneData.selectionID = currentIDCount;
            return 0;
        }
        else
        {
            int it = currentIDCount - 1;
            foreach (var child in node.children)
            {
                it++;
                int id = IDSelectionOptions(child, it);
                it += id;

            }
            return it;
        }
    }

    public int GetLastSelectionID()
    {
        return lastSelectionID;
    }
}
