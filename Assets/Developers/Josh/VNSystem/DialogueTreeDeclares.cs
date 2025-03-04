using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEditor.Animations;

public class TwineData
{
    public string title = "";
    public List<string> responseData = new List<string>();
}

public class DialogueTreeNode
{
    public DialogueTreeNode()
    {
        sceneData = null;
        twineData = new TwineData();
    }
    public DialogueTreeNode(VisualNovelScene scene)
    {
        sceneData = scene;
        twineData = new TwineData();
    }

    public DialogueTreeNode parent;
    public TwineData twineData = new TwineData();
    public List<DialogueTreeNode> children = new List<DialogueTreeNode>();
    public VisualNovelScene sceneData = new VisualNovelScene();

    public void AddChild(DialogueTreeNode newChild)
    {
        children.Add(newChild);
    }

    public bool isLeaf()
    {
        return !(children.Count > 0);
    }

    // public DialogueTreeNode FindNodeWithTitle(string title)
    // {
    //     DialogueTreeNode resultNode = null;
    //     if (twineData.Title == title)
    //     {
    //         resultNode = this;
    //         return resultNode;
    //     }
    //     else
    //     {
            
    //         foreach (DialogueTreeNode child in children)
    //         {
    //             resultNode = child.FindNodeWithTitle(title);
    //             if (resultNode != null)
    //             {
    //                 return resultNode;
    //             }

    //         }
    //     }

    //     UnityEngine.Debug.LogError("node with given title: " + title + " not found");
    //     return resultNode;
    // }
}

public class DialogueTree
{
    public DialogueTreeNode rootNode = new DialogueTreeNode();

    public DialogueTree()
    {
        rootNode = null;
    }
    public DialogueTree(DialogueTreeNode root)
    {
        rootNode = root;
    }

}

[System.Serializable]
public class SerializedTree
{
    public List<SerializedNode> nodes;

    public void Flattentree(DialogueTreeNode root)
    {
        nodes = new List<SerializedNode>();
        FlattenNode(root);
    }

    private void FlattenNode(DialogueTreeNode node)
    {
        nodes.Add(new SerializedNode(node));
        foreach (var child in node.children)
        {
            FlattenNode(child);
        }
    }
}

[System.Serializable]
public class SerializedNode
{
    public int id;
    public int parentId;
    public VisualNovelScene sceneData;

    public SerializedNode(DialogueTreeNode node)
    {
        id = node.GetHashCode();
        sceneData = node.sceneData;
        if (node.parent != null)
        {
            parentId = node.parent.GetHashCode();
        }
    }
}