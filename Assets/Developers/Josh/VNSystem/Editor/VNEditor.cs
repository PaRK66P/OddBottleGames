
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

//[CustomEditor(typeof(VisualNovelScript))]
public class VNEditor : Editor
{
    public VisualTreeAsset EditorXML;
    public override VisualElement CreateInspectorGUI()
    {
        //serializedObject.Update();
        VisualElement myInspector = new VisualElement();

        myInspector.Add(new Label("This is my custom editor"));

        EditorXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Developers/Josh/VNUIBuilderAsset.uxml");
        myInspector.Add(EditorXML.Instantiate());

        return myInspector;
    }

}

[CustomPropertyDrawer(typeof(VisualNovelScene))]
public class VNScenePropertyDraw : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement();

        var popup = new UnityEngine.UIElements.PopupWindow();
        popup.text = "Scene Details";
        //popup.Add(new PropertyField(property.FindPropertyRelative("text"), "text"));
        //popup.Add(new PropertyField(property.FindPropertyRelative("CharacterAsset"), "Character sprite"));
        container.Add(popup);

        return container;
    }
}

