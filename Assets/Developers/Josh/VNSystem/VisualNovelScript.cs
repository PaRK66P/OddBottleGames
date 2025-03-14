using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


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

    [SerializeField] GameObject playerRef;

    public bool isNovelSection;
    public string newtext;
    public GameObject canv;
    public GameObject text;
    public GameObject sprite;
    private CanvasGroup canvGroup;
    private GameObject playerUI;

    public Transform buttonContainer;
    public GameObject buttonPrefab;
    private List<GameObject> buttons = new List<GameObject>();

    DialogueTreeNode currentNode;
    int currentVNPrefabIndex = 0;
    private int lastSelectionID = 0;

    private IEnumerator typingText;

    [SerializeField]
    private bool fadeIn = false;
    [SerializeField]
    private bool fadeOut = false;

    public float typeTextSpeed = 1.0f;

    public bool typingTextToggle = true;

    public UnityEvent onNovelFinish;

    void Start()
    {
        canv = GameObject.Find("Canvas").transform.Find("VisualNovelCanvas").gameObject;
        text = canv.transform.Find("VisualNovelText").gameObject;
        sprite = canv.transform.Find("VisualNovelSprite").gameObject;
        buttonContainer = canv.transform.Find("VisualNovelButtonContainer").GetComponent<Transform>();
        canvGroup = canv.GetComponent<CanvasGroup>();
        playerUI = GameObject.Find("Canvas").transform.Find("PlayerUI").gameObject;

        GameObject[] VisualNovelPrefabs = Resources.LoadAll<GameObject>("VisualNovelScenes");
        foreach (GameObject prefab in VisualNovelPrefabs)
        {
            if (prefab != null)
            {
                VNPrefabScript script = Instantiate(prefab).GetComponent<VNPrefabScript>();
                if (script != null)
                {
                    VNScenes.Add(prefab.GetComponent<VNPrefabScript>());
                    //script.VNname = prefab.name;
                }
            }
        }
    }

    private void Update()
    {
        if (fadeIn)
        {
            canv.SetActive(true);
            canvGroup.alpha += Time.unscaledDeltaTime*2;
            if (canvGroup.alpha >= 1.0f)
            {
                fadeIn = false;
            }
        }

        if (fadeOut && !fadeIn)
        {
            canvGroup.alpha -= Time.unscaledDeltaTime*2;
            if (canvGroup.alpha <= 0.0f)
            {
                fadeOut = false;
                canv.SetActive(false);
            }
            
        }
        
    }

    public void StartNovelScene(int NovelSceneID)
    {
        if (!isNovelSection)
        {
            canvGroup.alpha = 0;
            Time.timeScale = 0;
            playerRef.GetComponent<PlayerManager>().DisableInput();
            currentVNPrefabIndex = NovelSceneID;
            playerUI.SetActive(false);

            isNovelSection = true;

            //canv.SetActive(true);

            fadeIn = true;

            if (currentVNPrefabIndex < VNScenes.Count && currentVNPrefabIndex > -1)
            {
                DialogueTree tree = new DialogueTree(ReconstructTree(VNScenes[currentVNPrefabIndex].tree));
                currentNode = tree.rootNode;
                if (typingTextToggle == true)
                {
                    typingText = TypewriterText(currentNode.sceneData.text);
                    StartCoroutine(typingText);
                }
                else
                {
                    text.GetComponent<TMP_Text>().text = currentNode.sceneData.text;
                }
                sprite.GetComponent<Image>().sprite = currentNode.sceneData.CharacterAsset;
                sprite.GetComponent<Image>().SetNativeSize();

                int count = -1;
                IDSelectionOptions(currentNode, ref count);
                CreateButtons();
            }
            else
            {
                isNovelSection = false;
                Debug.LogError("Invalid Novel Scene ID");
            }
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
        ClearButtons();
        //StopCoroutine(typingText);
        if (!currentNode.isLeaf())
        {
            if (index > -1 && index < currentNode.children.Count)
            {
                currentNode = currentNode.children[index];
                sprite.GetComponent<Image>().sprite = currentNode.sceneData.CharacterAsset;
                if (typingTextToggle)
                {
                    typingText = TypewriterText(currentNode.sceneData.text);
                    StartCoroutine(typingText);
                }
                else
                {
                    text.GetComponent<TMP_Text>().text = currentNode.sceneData.text;
                }
                //CreateButtons();
            }
            else
            {
                Debug.LogError("tried to transition to invalid scene index");
            }
        }
        else
        {

            lastSelectionID = currentNode.sceneData.selectionID;
            isNovelSection = false;
            playerRef.GetComponent<PlayerManager>().EnableInput();
            Time.timeScale = 1.0f;
            fadeOut = true;
            //canv.SetActive(false);
            playerUI.SetActive(true);
            if (onNovelFinish != null)
            {
                onNovelFinish?.Invoke();
            }

        }
    }

    public DialogueTreeNode ReconstructTree(SerializedTree serializedTree)
    {
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

    public void ClearButtons()
    {
        foreach (GameObject button in buttons)
        {
            Destroy(button);
        }
        buttons.Clear();
    }
    public void CreateButtons()
    {
        ClearButtons();

        if (currentNode.isLeaf())
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
            newButton.name = "Button_" + 0;
            //newButton.transform.localScale = new Vector3(2, 2, 1);

            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
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
            //newButton.GetComponent<RectTransform>().offsetMin = new Vector2(0, -15);
            //newButton.GetComponent<RectTransform>().offsetMax = new Vector2(250, 0);
            //newButton.transform.localScale = new Vector3(2, 2, 1);


            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = currentNode.children[i].sceneData.entryText;
                buttonText.fontSize = 12;
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

    void IDSelectionOptions(DialogueTreeNode node, ref int currentIDCount)
    {
        if (node == null)
            Debug.LogError("null node when assigning IDs");

        if (node.isLeaf())
        {
            node.sceneData.selectionID = ++currentIDCount;
        }
        else
        {
            foreach (var child in node.children)
            {
                IDSelectionOptions(child, ref currentIDCount);
            }
        }
    }

    public int GetLastSelectionID()
    {
        return lastSelectionID;
    }

    private IEnumerator TypewriterText(string targetText)
    {
        string textToAdd = "";
        TMP_Text TMP = text.GetComponent<TMP_Text>();
        for (int i = 0; i < targetText.Length; i++)
        {
            textToAdd += targetText[i];
            TMP.text = textToAdd;
            
            yield return new WaitForSecondsRealtime(0.05f / typeTextSpeed);
            if (Input.GetMouseButton(0))
            {
                TMP.text = targetText;
                break;
            }
        }
        CreateButtons();
        yield return null;
    }
}
