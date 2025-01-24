using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class VNEditorWindow : EditorWindow
{
    //keep track of index we have selected for hot reloads
    [SerializeField]
    private int selectedIndex = -1;
    
    private VisualElement rightPane;

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
        var allObjectGuids = AssetDatabase.FindAssets("t:Sprite");
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

        //add a list to the left pane and a scrolling viewport to the right pane
        var leftPane = new ListView();
        splitView.Add(leftPane);
        rightPane = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
        splitView.Add(rightPane);

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
        rightPane.Clear();

        //check that the current sprite is valid and exists
        var enumerator = selectedItems.GetEnumerator();
        if (enumerator.MoveNext())
        {
            var selectedSprite = enumerator.Current as Sprite;
            if (selectedSprite != null)
            {
                //if we find a sprite we create an image and add it to the right viewport
                var spriteImage = new Image();
                spriteImage.scaleMode = ScaleMode.ScaleToFit;
                spriteImage.sprite = selectedSprite;

                rightPane.Add(spriteImage);
            }
        }
    }
}
