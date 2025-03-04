using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine.UI;



public class VNEditorWindow : EditorWindow
{
    //keep track of index we have selected for hot reloads
    [SerializeField]
    private int selectedIndex = -1;

    private int width = 852;
    private int height = 480;

    private ScrollView graphPane;
    private VisualElement topRightPane;
    private TextField bottomRightPane;
    private TwoPaneSplitView graphSplitView;
    private IntegerField nextSceneIndexInput;
    private TextField entryTextField;

    [SerializeField]
    private string textFieldInput = "";

    [SerializeField]
    private int integerFieldInput = 0;

    [SerializeField]
    private string titleString = "";

    [SerializeField]
    private string entryTextString = "";

    [SerializeField]
    private int fontSize = 18;

    Sprite selectedSprite;
    Sprite defaultSprite;

    private DialogueTreeNode workingRoot = new DialogueTreeNode(new VisualNovelScene());
    private DialogueTreeNode currentNode;

    //should always end with /VisualNovelScenes else problems will ensue - also needs to be changed in VisualNovelScript cause im lazy
    const string PrefabFolderPath = "Assets/Resources/VisualNovelScenes";


    //tag as menu item
    [MenuItem("Window/UI Toolkit/Visual Novel Scene Editor")]

    //showMyEditor creates a window initially
    public static void ShowMyEditor()
    {
        EditorWindow wnd = GetWindow<VNEditorWindow>();
        wnd.titleContent = new GUIContent("Visual Novel Scene Editor");

        //define max and min sizes
        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1400, 900);


    }

    //CreateGUI functions similar to update
    public void CreateGUI()
    {
        if (currentNode == null)
        {
            currentNode = workingRoot;
        }
        //enumerate all sprites in the project
        var allObjectGuids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/Developers/Josh/Sprites" });
        var allObjects = new List<Sprite>();
        foreach (var guid in allObjectGuids)
        {
            allObjects.Add(AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid)));
        }

        if (defaultSprite == null)
        {
            foreach (Sprite sprite in allObjects)
            {
                if (sprite.name == "missing sprite_0")
                {
                    defaultSprite = sprite;
                }
            }
        }

        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        TextField titleField = new TextField("Scene Name");
        titleField.value = titleString;
        titleField.RegisterValueChangedCallback(evt =>
        {
            titleString = evt.newValue;
            UpdateViewPort();
        });
        rootVisualElement.Add(titleField);

        entryTextField = new TextField("Entry Text");
        entryTextField.value = entryTextString;
        entryTextField.RegisterValueChangedCallback(evt =>
        {
            entryTextString = evt.newValue;
            UpdateViewPort();
        });
        rootVisualElement.Add(entryTextField);

        graphSplitView = new TwoPaneSplitView(0, 700, TwoPaneSplitViewOrientation.Vertical);
        rootVisualElement.Add(graphSplitView);

        //create a splitview for two panes
        var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
        graphSplitView.Add(splitView);

        graphPane = GenerateTreeDiagram();
        //graphPane.style.flexDirection = FlexDirection.Column;
        graphSplitView.Add(graphPane);

        //add a listview to the left pane of the split view
        var leftPane = new ListView();
        splitView.Add(leftPane);

        //vertical splitview for right side
        var rightSplitView = new TwoPaneSplitView(0, 500, TwoPaneSplitViewOrientation.Vertical);
        splitView.Add(rightSplitView);

        //add scrolling viewport to the top of the right side split view
        topRightPane = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
        rightSplitView.Add(topRightPane);

        TwoPaneSplitView bottomRightSplit = new TwoPaneSplitView(0, 600, TwoPaneSplitViewOrientation.Horizontal);
        rightSplitView.Add(bottomRightSplit);

        //styling for text input field
        bottomRightPane = new TextField("Dialogue");
        bottomRightPane.multiline = true;
        bottomRightPane.style.whiteSpace = WhiteSpace.Normal;
        bottomRightPane.style.minHeight = EditorGUIUtility.singleLineHeight * 10.0f;

        var textInputField = bottomRightPane.Q("unity-text-input");
        textInputField.style.height = EditorGUIUtility.singleLineHeight * 10.0f;
        textInputField.style.whiteSpace = WhiteSpace.Normal;

        //Debug.Log(textFieldInput);
        bottomRightSplit.Add(bottomRightPane);
        bottomRightPane.value = textFieldInput;
        bottomRightPane.RegisterValueChangedCallback(evt =>
        {
            textFieldInput = evt.newValue;
            UpdateViewPort();
        });

        VisualElement ButtonContainer = new VisualElement();
        bottomRightSplit.Add(ButtonContainer);

        var nextSceneButtonPanel = new VisualElement();
        ButtonContainer.Add(nextSceneButtonPanel);

        nextSceneIndexInput = new IntegerField("next node index");
        nextSceneIndexInput.value = integerFieldInput;
        nextSceneIndexInput.RegisterValueChangedCallback(evt => { integerFieldInput = evt.newValue; });
        nextSceneButtonPanel.Add(nextSceneIndexInput);

        var nextSceneButton = new UnityEngine.UIElements.Button();
        nextSceneButton.text = "Next Scene";
        nextSceneButton.clicked += OnNextSceneClick;
        nextSceneButtonPanel.Add(nextSceneButton);

        var prevSceneButton = new UnityEngine.UIElements.Button();
        prevSceneButton.text = "Previous Scene";
        prevSceneButton.clicked += OnPrevSceneClick;
        ButtonContainer.Add(prevSceneButton);

        var newBranchButton = new UnityEngine.UIElements.Button();
        newBranchButton.text = "New Branch";
        newBranchButton.clicked += CreateNewScene;
        ButtonContainer.Add(newBranchButton);

        var compileButton = new UnityEngine.UIElements.Button();
        compileButton.text = "Generate Scene";
        compileButton.clicked += OnGenerateClick;
        ButtonContainer.Add(compileButton);

        var loadTwineButton = new UnityEngine.UIElements.Button();
        loadTwineButton.text = "Load Twine File";
        loadTwineButton.clicked += LoadInTwineScene;
        ButtonContainer.Add(loadTwineButton);




        //bind the enumerated list of sprites to the left pane
        leftPane.makeItem = () => new Label();
        leftPane.bindItem = (item, index) => { (item as Label).text = allObjects[index].name; };
        leftPane.itemsSource = allObjects;

        //set up listeners for events to update on selection
        leftPane.selectionChanged += OnSpriteSelectionChange;
        leftPane.selectedIndex = selectedIndex;
        leftPane.selectionChanged += (items) => { selectedIndex = leftPane.selectedIndex; };
    }

    public void Update()
    {
        bottomRightPane.value = textFieldInput;
        entryTextField.value = entryTextString;

    }

    private void UpdateViewPort()
    {
        if (selectedSprite != null)
        {
            //clear the viewport
            topRightPane.Clear();

            //create a rendertexture for display and a temporary texture
            RenderTexture renderTexture = new RenderTexture(width, height, 24);
            RenderTexture.active = renderTexture;
            GL.Clear(true, true, Color.black);
            Texture2D newTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

            //create temp gameobject for the canvas and canvas scaler
            GameObject tempGO = new GameObject("TempCanv");
            tempGO.transform.position = Vector2.zero;
            Canvas canvas = tempGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.sortingOrder = 1;
            CanvasScaler canvasScaler = tempGO.AddComponent<CanvasScaler>();
            canvasScaler.referenceResolution = new Vector2(width, height);
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 0.5f;
            canvasScaler.scaleFactor = 1.0f;
            canvasScaler.dynamicPixelsPerUnit = 1;

            //create temporary camera and add to same game object as canvas (initialised like default unity camera)
            GameObject camGO = new GameObject("TempCam");
            Camera camera = camGO.AddComponent<Camera>();
            camera.cullingMask = -1;
            camera.orthographic = true;
            camera.nearClipPlane = 0.3f;
            camera.farClipPlane = 1000.0f;
            camera.orthographicSize = 256.0f;
            camera.targetTexture = renderTexture;
            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = Color.black;
            camGO.transform.position = new Vector3(0, 0, -20);
            camera.depth = 1;

            //set canvas worldcamera as camera
            canvas.worldCamera = camera;

            //create temporary gameobject for text as a child of the canvas object
            GameObject textGO = new GameObject("Text");
            textGO.transform.SetParent(canvas.transform);
            Text textComponent = textGO.AddComponent<Text>();
            textComponent.fontSize = fontSize;
            textComponent.color = Color.white;
            textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textComponent.text = textFieldInput;
            textComponent.alignment = TextAnchor.UpperLeft;

            //create canvas renderer for text component
            CanvasRenderer textCanvasrenderer = textGO.GetComponent<CanvasRenderer>();
            textCanvasrenderer.cullTransparentMesh = true;

            //avoiding z position being set to -4608 for reasons only god and satan understand (and also set size of the text box and stuff)
            RectTransform textRectTransform = textGO.GetComponent<RectTransform>();
            textRectTransform.position = Vector3.zero;
            textRectTransform.sizeDelta = new Vector2(width - 50, 100);
            textRectTransform.anchoredPosition = new Vector2(0, 0);

            textGO.transform.position = new Vector3(0, -height / 2 + 100, 0);

            //create a simple background image in the same place as the text
            GameObject backgroundImageGO = new GameObject("Background Image");
            backgroundImageGO.transform.SetParent(canvas.transform);
            UnityEngine.UI.Image bgImage = backgroundImageGO.AddComponent<UnityEngine.UI.Image>();
            bgImage.color = new Color(0.3f, 0.3f, 0.3f, 1.0f);
            RectTransform bgImageRectTransform = backgroundImageGO.GetComponent<RectTransform>();
            bgImageRectTransform.position = textRectTransform.position;
            bgImageRectTransform.sizeDelta = new Vector2(textRectTransform.sizeDelta.x + 10, textRectTransform.sizeDelta.y + 10);
            bgImageRectTransform.anchoredPosition = textRectTransform.anchoredPosition;

            backgroundImageGO.transform.position = new Vector3(textGO.transform.position.x, textGO.transform.position.y, -10.0f);

            textRectTransform.SetSiblingIndex(bgImageRectTransform.GetSiblingIndex() + 1);

            //create image to load sprite onto 
            GameObject imageGO = new GameObject("Image");
            imageGO.transform.SetParent(canvas.transform);
            UnityEngine.UI.Image character = imageGO.AddComponent<UnityEngine.UI.Image>();

            character.sprite = selectedSprite;

            //scaling
            float maxScale = 150.0f;
            float spriteWidth = character.sprite.rect.width;
            float spriteHeight = character.sprite.rect.height;
            float widthScale = maxScale / spriteWidth;
            float heightScale = maxScale / spriteHeight;
            float scale = Mathf.Min(widthScale, heightScale);
            character.rectTransform.position = Vector3.zero;
            character.rectTransform.sizeDelta = new Vector2(spriteWidth * scale, spriteHeight * scale);
            character.rectTransform.anchoredPosition = new Vector2(0, 0);

            imageGO.transform.position = new Vector3(-width / 2 + 150, 0, 0);

            //update canvas
            Canvas.ForceUpdateCanvases();

            //render camera
            camera.Render();

            //read pixels and apply to texture
            newTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            newTexture.Apply();

            //create an image to display the texture we just created
            var image = new UnityEngine.UIElements.Image();
            image.scaleMode = ScaleMode.ScaleToFit;
            image.image = newTexture;
            topRightPane.Add(image);

            //destroy temp objects and reset viewport
            RenderTexture.active = null;
            Object.DestroyImmediate(tempGO);
            Object.DestroyImmediate(textGO);
            Object.DestroyImmediate(camGO);
            Object.DestroyImmediate(backgroundImageGO);
            renderTexture.Release();

            nextSceneIndexInput.SetValueWithoutNotify(integerFieldInput);
        }
    }
    //runs when a new sprite is selected
    private void OnSpriteSelectionChange(IEnumerable<object> selectedItems)
    {

        //check that the current sprite is valid and exists
        var enumerator = selectedItems.GetEnumerator();
        if (enumerator.MoveNext())
        {
            selectedSprite = enumerator.Current as Sprite;
            UpdateViewPort();

        }
    }

    private void OnGenerateClick()
    {
        //save to file directory as a prefab
        if (!AssetDatabase.IsValidFolder(PrefabFolderPath))
        {
            Debug.Log("creating new folder");
            //janky way of saying create a VisualNovelScenes folder at prefab folder path - /VisualNovelScenes (17 characters) - does mean that prefab path should always end with /VisualNovelScenes
            AssetDatabase.CreateFolder(PrefabFolderPath.Substring(0, PrefabFolderPath.Length - 18), "VisualNovelScenes");
        }

        if (titleString != "")
        {
            if (IsValidSceneName(titleString))
            {
                //create new prefab and add script to it
                GameObject newScenePrefab = new GameObject(titleString);
                VNPrefabScript newPrefabScript = newScenePrefab.AddComponent<VNPrefabScript>();

                if (newPrefabScript == null)
                {
                    Debug.LogError("Prefab Script not attached to prefab or gameobject");
                    return;
                }
                //fill in data
                currentNode.sceneData = new VisualNovelScene(selectedSprite, textFieldInput, entryTextString);
                DialogueTree newTree = new DialogueTree(workingRoot);
                newPrefabScript.SetScene(newTree);

                //always save prefab after making all changes to script or they wont be saved
                string prefabPath = PrefabFolderPath + "/" + titleString + ".prefab";
                PrefabUtility.SaveAsPrefabAsset(newScenePrefab, prefabPath);

                Debug.Log("saving scene as: " + name);
                //clean up after
                DestroyImmediate(newScenePrefab);

            }
            else
            {
                Debug.LogError("Scene already exists with this name");
            }
        }
        else
        {
            Debug.LogError("Cannot save scene without name");
        }
    }


    //enumerate all prefabs in prefab folder and check none have the same name as we are checking
    private bool IsValidSceneName(string name)
    {
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { PrefabFolderPath });

        if (prefabGUIDs.Length > 0)
        {
            foreach (string guid in prefabGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath);

                if (assetName == name)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void OnNextSceneClick()
    {
        if (!currentNode.isLeaf())
        {
            if (entryTextString != "")
            {


                VisualNovelScene sceneData = new VisualNovelScene();
                sceneData.text = textFieldInput;
                sceneData.CharacterAsset = selectedSprite;
                sceneData.entryText = entryTextString;
                currentNode.sceneData = sceneData;

                if (integerFieldInput < currentNode.children.Count && integerFieldInput > -1)
                {
                    currentNode = currentNode.children[integerFieldInput];
                    selectedSprite = currentNode.sceneData.CharacterAsset;
                    textFieldInput = currentNode.sceneData.text;
                    entryTextString = currentNode.sceneData.entryText;
                    integerFieldInput = 0;
                    UpdateGraphPane();
                }
                else
                {
                    Debug.LogError("next node ID out of range - IDs start at 0");
                }
            }
            else
            {
                Debug.LogError("must write entry text to move to next scene");
            }
        }
        else
        {
            CreateNewScene();
        }
        UpdateViewPort();
    }

    private void CreateNewScene()
    {
        if (entryTextString != "")
        {
            VisualNovelScene sceneData = new VisualNovelScene();
            sceneData.text = textFieldInput;
            sceneData.CharacterAsset = selectedSprite;
            sceneData.entryText = entryTextString;
            currentNode.sceneData = sceneData;
            DialogueTreeNode newChild = new DialogueTreeNode();
            newChild.parent = currentNode;
            VisualNovelScene newNodeScene = new VisualNovelScene(defaultSprite, "", "");
            currentNode.AddChild(newChild);
            integerFieldInput = currentNode.children.Count - 1;
            currentNode = newChild;

            selectedSprite = defaultSprite;
            textFieldInput = "";
            entryTextString = "";


            UpdateGraphPane();
            UpdateViewPort();
        }
        else
        {
            Debug.LogError("must write entry text to create a new node");
        }

    }

    private void OnPrevSceneClick()
    {
        if (currentNode.parent != null)
        {
            VisualNovelScene sceneData = new VisualNovelScene();
            sceneData.text = textFieldInput;
            sceneData.CharacterAsset = selectedSprite;
            sceneData.entryText = entryTextString;
            currentNode.sceneData = sceneData;
            currentNode = currentNode.parent;
            integerFieldInput = 0;
            selectedSprite = currentNode.sceneData.CharacterAsset;
            textFieldInput = currentNode.sceneData.text;
            entryTextString = currentNode.sceneData.entryText;

            UpdateGraphPane();
            UpdateViewPort();
        }
    }

    private List<(Vector2 lineStart, Vector2 lineEnd)> lines = new List<(Vector2 lineStart, Vector2 lineEnd)>();
    private ScrollView GenerateTreeDiagram()
    {
        lines.Clear();
        ScrollView treeRoot = new ScrollView(ScrollViewMode.Vertical);

        treeRoot.style.width = 1400;
        treeRoot.style.height = 1000;

        if (workingRoot == null)
        {
            return treeRoot;
        }


        treeRoot.generateVisualContent += ctx => DrawLines(ctx);
        float startingYPos = GetSubTreeHeight(workingRoot, 20);
        DrawTreeNode(treeRoot, workingRoot, new Vector2(50, startingYPos), "root");


        return treeRoot;
    }

    private void DrawTreeNode(VisualElement nodeDrawingCanvas, DialogueTreeNode node, Vector2 position, string nodeName)
    {
        VisualElement nodeDrawing = new VisualElement();
        nodeDrawing.style.width = 50;
        nodeDrawing.style.height = 50;
        nodeDrawing.style.position = Position.Absolute;
        if (node == currentNode)
        {
            nodeDrawing.style.color = new StyleColor(Color.red);
        }
        else
        {
            nodeDrawing.style.color = new StyleColor(Color.white);
        }
        nodeDrawing.style.transformOrigin = nodeDrawingCanvas.style.transformOrigin;
        nodeDrawing.style.marginBottom = 10;
        nodeDrawing.style.translate = new Translate(position.x, position.y);

        var nodeLabel = new Label(nodeName);
        nodeLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        nodeLabel.style.backgroundColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f, 0.5f));
        nodeDrawing.Add(nodeLabel);

        float totalChildHeight = 0.0f;
        foreach (var child in node.children)
        {
            totalChildHeight += GetSubTreeHeight(child, 20) - 10;
        }
        float newYPos = position.y - totalChildHeight;
        float index = 0;
        foreach (var child in node.children)
        {
            float subTreeHeight = GetSubTreeHeight(child, 20);
            float childPositionY = newYPos + (subTreeHeight / 2.0f);

            lines.Add((new Vector2(position.x + 50.0f, position.y + 8.0f),
                new Vector2(position.x + 60.0f, childPositionY + 8.0f)));


            DrawTreeNode(nodeDrawingCanvas, child, new Vector2(position.x + 60, childPositionY), index.ToString());
            newYPos += subTreeHeight;
            index++;
        }

        nodeDrawingCanvas.Add(nodeDrawing);
    }

    private float GetSubTreeHeight(DialogueTreeNode node, float nodeHeight)
    {
        if (node.isLeaf())
        {
            return nodeHeight;
        }

        float height = 0;
        foreach (var child in node.children)
        {
            height += GetSubTreeHeight(child, nodeHeight);
        }
        return height;
    }

    private void DrawLines(MeshGenerationContext mgc)
    {
        Vector2 scrollOffset = (mgc.visualElement as ScrollView)?.scrollOffset ?? Vector2.zero;

        foreach (var (start, end) in lines)
        {
            Vector2 adjustedStart = start - scrollOffset;
            Vector2 adjustedEnd = end - scrollOffset;


            var painter = mgc.painter2D;
            painter.strokeColor = Color.gray;
            painter.lineWidth = 2;
            painter.BeginPath();
            painter.MoveTo(adjustedStart);
            painter.LineTo(adjustedEnd);
            painter.Stroke();
        }
    }

    private void UpdateGraphPane()
    {
        graphPane.parent.Remove(graphPane);
        graphPane = GenerateTreeDiagram();
        graphSplitView.Add(graphPane);
    }

    private void LoadInTwineScene()
    {
        TextAsset myTwineData = Resources.Load("TwineFiles/DateorDecapitate2") as TextAsset;
        //Debug.Log(myTwineData.text);

        DialogueTree twineTree = TwineParser.ConstructTwineTree(TwineParser.ParseTwineText(myTwineData.text));

        workingRoot = twineTree.rootNode;
        currentNode = twineTree.rootNode;

        entryTextString = workingRoot.twineData.title;
        textFieldInput = workingRoot.sceneData.text;

        UpdateGraphPane();
    }
}
