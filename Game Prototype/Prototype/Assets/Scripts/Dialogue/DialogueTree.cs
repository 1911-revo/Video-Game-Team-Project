using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Tree")]
public class DialogueTree : ScriptableObject
{
    [SerializeField] public List<DialogueNode> nodes = new List<DialogueNode>();
    [SerializeField] public string startNodeID;

    // Helper to find a node by ID
    public DialogueNode GetNodeByID(string id)
    {
        return nodes.Find(node => node.nodeID == id);
    }

    // Get starting node
    public DialogueNode GetStartNode()
    {
        return GetNodeByID(startNodeID);
    }
}