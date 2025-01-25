using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor.UIElements;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using System.Runtime.CompilerServices;
using NUnit.Framework.Constraints;
using UnityEditor.PackageManager.Requests;

public class VNEditorWindow : EditorWindow
{
    //keep track of index we have selected for hot reloads
    [SerializeField]
    private int selectedIndex = -1;

    private int width = 852;
    private int height = 480;

    private VisualElement topRightPane;
    private TextField bottomRightPane;

    [SerializeField]
    private string textFieldInput = "";

    [SerializeField]
    private string titleString = "";

    [SerializeField]
    private int fontSize = 18;

    Sprite selectedSprite;


    //tag as menu item
    [MenuItem("Window/UI Toolkit/Visual Novel Scene Editor")]

    //showMyEditor creates a window initially
    public static void ShowMyEditor()
    {
        EditorWindow wnd = GetWindow<VNEditorWindow>();
        wnd.titleContent = new GUIContent("Visual Novel Scene Editor");

        //define max and min sizes
        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1920, 720);
    }

    //CreateGUI functions similar to update
    public void CreateGUI()
    {
        //enumerate all sprites in the project
        var allObjectGuids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/Developers/Josh/Sprites" });
        var allObjects = new List<Sprite>();
        foreach (var guid in allObjectGuids)
        {
            allObjects.Add(AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid)));
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

        //create a splitview for two panes
        var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
        rootVisualElement.Add(splitView);

        //add a listview to the left pane of the split view
        var leftPane = new ListView();
        splitView.Add(leftPane);

        //vertical splitview for right side
        var rightSplitView = new TwoPaneSplitView(0, 500, TwoPaneSplitViewOrientation.Vertical);
        splitView.Add(rightSplitView);

        //add scrolling viewport to the top of the right side split view
        topRightPane = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
        rightSplitView.Add(topRightPane);

        //styling for text input field
        bottomRightPane = new TextField("Dialogue");
        bottomRightPane.multiline = true;
        bottomRightPane.style.whiteSpace = WhiteSpace.Normal;
        bottomRightPane.style.minHeight = EditorGUIUtility.singleLineHeight * 10.0f;

        var textInputField = bottomRightPane.Q("unity-text-input");
        textInputField.style.height = EditorGUIUtility.singleLineHeight * 10.0f;
        textInputField.style.whiteSpace = WhiteSpace.Normal;

        rightSplitView.Add(bottomRightPane);
        bottomRightPane.value = textFieldInput;
        bottomRightPane.RegisterValueChangedCallback(evt =>
        {
            textFieldInput = evt.newValue;
            UpdateViewPort();
        });

        var compileButton = new UnityEngine.UIElements.Button();
        compileButton.text = "Generate Scene";
        compileButton.clicked += OnGenerateClick;
        bottomRightPane.Add(compileButton);


        //bind the enumerated list of sprites to the left pane
        leftPane.makeItem = () => new Label();
        leftPane.bindItem = (item, index) => { (item as Label).text = allObjects[index].name; };
        leftPane.itemsSource = allObjects;

        //set up listeners for events to update on selection
        leftPane.selectionChanged += OnSpriteSelectionChange;
        leftPane.selectedIndex = selectedIndex;
        leftPane.selectionChanged += (items) => { selectedIndex = leftPane.selectedIndex; };


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
            float spriteWidth = selectedSprite.rect.width;
            float spriteHeight = selectedSprite.rect.height;
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
        if (!AssetDatabase.IsValidFolder("Assets/Developers/Josh/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets/Developers/Josh", "Prefabs");
        }
        if (titleString != "")
        {
            if (IsValidSceneName(titleString))
            {
                //create new prefab and add script to it
                GameObject newScenePrefab = new GameObject(titleString);
                VNPrefabScript newPrefabScript = newScenePrefab.AddComponent<VNPrefabScript>();

                //fill in data
                VisualNovelScene sceneData = new VisualNovelScene(selectedSprite, textFieldInput);
                newPrefabScript.Scenes.Add(sceneData);

                string prefabPath = "Assets/Developers/Josh/Prefabs/" + titleString + ".prefab";
                PrefabUtility.SaveAsPrefabAsset(newScenePrefab, prefabPath);

                //clean up after
                DestroyImmediate(newScenePrefab);
            }
            else
            {
                Debug.LogError("Invalid scene name");
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
        
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Developers/Josh/Prefabs" });
        
        foreach (string guid in prefabGUIDs)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath);

            if (assetName == name)
            {
                return false;
            }
        }
        return true;
    }
}
