using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor.UIElements;

public class VNEditorWindow : EditorWindow
{
    //keep track of index we have selected for hot reloads
    [SerializeField]
    private int selectedIndex = -1;
    
    private VisualElement topRightPane;
    private TextField bottomRightPane;

    [SerializeField]
    private string textFieldInput = "";

    [SerializeField]
    private int fontSize = 18;

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
        var allObjectGuids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/Developers/Josh/Sprites" } );
        var allObjects = new List<Sprite>();
        foreach (var guid in allObjectGuids)
        {
            allObjects.Add(AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid)));
        }

        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

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
        bottomRightPane.RegisterValueChangedCallback(evt => { textFieldInput = evt.newValue; });

        //bind the enumerated list of sprites to the left pane
        leftPane.makeItem = () => new Label();
        leftPane.bindItem = (item, index) => { (item as Label).text = allObjects[index].name; };
        leftPane.itemsSource = allObjects;

        //set up listeners for events to update on selection
        leftPane.selectionChanged += OnSpriteSelectionChange;
        leftPane.selectedIndex = selectedIndex;
        leftPane.selectionChanged += (items) => { selectedIndex = leftPane.selectedIndex; };
        
    }

    //runs when a new sprite is selected
    private void OnSpriteSelectionChange(IEnumerable<object> selectedItems)
    {
        //clear the right pane at the start
        topRightPane.Clear();

        //check that the current sprite is valid and exists
        var enumerator = selectedItems.GetEnumerator();
        if (enumerator.MoveNext())
        {
            var selectedSprite = enumerator.Current as Sprite;
            if (selectedSprite != null)
            {
                //create a rendertexture for display and a temporary texture
                RenderTexture renderTexture = new RenderTexture(512, 512, 24);
                RenderTexture.active = renderTexture;
                GL.Clear(true, true, Color.blue);
                Texture2D newTexture = new Texture2D(512, 512, TextureFormat.RGBA32, false);

                //copy sprite to new texture
                Graphics.Blit(selectedSprite.texture, renderTexture);
                newTexture.ReadPixels(new Rect(0, 0, 512, 512), 0, 0);
                newTexture.Apply();

                //create temp gameobject for the canvas and canvas scaler
                GameObject tempGO = new GameObject("TempCanv");
                tempGO.transform.position = Vector2.zero;
                Canvas canvas = tempGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                CanvasScaler canvasScaler = tempGO.AddComponent<CanvasScaler>();
                canvasScaler.referenceResolution = new Vector2(512, 512);
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
                camera.clearFlags = CameraClearFlags.Skybox;
                camera.backgroundColor = Color.blue;
                camGO.transform.position = new Vector3(0, 0, -20);
                camera.depth = -1;

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
                textComponent.alignment = TextAnchor.MiddleCenter;
                
                CanvasRenderer textCanvasrenderer = textGO.GetComponent<CanvasRenderer>();
                textCanvasrenderer.cullTransparentMesh = true;

                textGO.GetComponent<RectTransform>().position = Vector3.zero;

                //RectTransform rectTransform = textGO.GetComponent<RectTransform>();
                //rectTransform.sizeDelta = new Vector2(512, 512);
                //rectTransform.anchoredPosition = Vector2.zero;
                //rectTransform.anchorMin = Vector2.zero;
                //rectTransform.anchorMax = Vector2.zero;

                //update canvas
                Canvas.ForceUpdateCanvases();

                //render camera
                camera.Render();

                //read pixels and apply to texture
                newTexture.ReadPixels(new Rect(0, 0, 512, 512), 0, 0);
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
                renderTexture.Release();
            }
        }
    }
}
