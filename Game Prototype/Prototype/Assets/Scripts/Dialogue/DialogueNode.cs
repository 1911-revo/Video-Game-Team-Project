using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueNode
{
    [SerializeField] public string nodeID;
    [SerializeField] public string text;
    [SerializeField] public List<DialogueChoice> choices;
    [SerializeField] public string nextNodeID; // Reference to the next node if there are no dialogue choices.

    [SerializeField] public int valueWhenReached; // How to effect the ending score when this node is reached.

    [Serializable]
    public class DialogueChoice
    {
        [SerializeField] public string choiceText;
        [SerializeField] public string targetNodeID;
    }
}