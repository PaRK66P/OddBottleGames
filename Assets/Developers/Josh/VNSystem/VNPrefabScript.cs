using UnityEngine;

public class VNPrefabScript : MonoBehaviour
{
    public SerializedTree tree;
    public void SetScene(DialogueTree newTree)
    {
        tree = new SerializedTree();
        tree.Flattentree(newTree.rootNode);
    }
}
