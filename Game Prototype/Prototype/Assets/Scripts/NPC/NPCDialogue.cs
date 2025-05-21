using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    // Not a great way of doing this, just tryna keep backwards compatability so I don't need to rewrite too much.
    [SerializeField] public List<string> dialogue;
    [SerializeField] public List<string> repeat;

    // Add new fields for branching dialogue
    [SerializeField] private DialogueTree dialogueTree;

    private bool hasSpokenBefore = false;

    // Method to check if this NPC uses branching dialogue
    public bool UsesBranchingDialogue()
    {
        return dialogueTree != null;
    }

    public DialogueTree GetDialogueTree()
    {
        return dialogueTree;
    }

    public string[] GetRepeatDialogue()
    {
        return repeat.ToArray();
    }

    public bool HasSpokenBefore()
    {
        return hasSpokenBefore;
    }

    public void SetSpokenBefore()
    {
        hasSpokenBefore = true;
    }
}