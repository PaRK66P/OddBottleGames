using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class VNEditorWindow : EditorWindow
{
    [MenuItem("Window/UI Toolkit/Visual Novel Scene Editor")]
    public static void ShowExample()
    {
        EditorWindow wnd = GetWindow<VNEditorWindow>();
        wnd.titleContent = new GUIContent("Visual Novel Scene Editor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

    }
}
