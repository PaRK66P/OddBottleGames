using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Hierarchy;
using UnityEngine;

public class DialogueTreeNode
{
    public DialogueTreeNode()
    {
        sceneData = null;
    }
    public DialogueTreeNode(VisualNovelScene scene)
    {
        sceneData = scene;
    }

    public DialogueTreeNode parent;
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

    public void Flattentree (DialogueTreeNode root)
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