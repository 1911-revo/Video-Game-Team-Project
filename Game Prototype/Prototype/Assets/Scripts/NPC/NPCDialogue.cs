using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [Header("Linear Dialogue System")]
    public List<string> dialogue = new List<string>();
    public List<string> repeat = new List<string>();

    [Header("Branching Dialogue System")]
    [SerializeField] public bool useDialogueTree = false;
    [SerializeField] public DialogueTree dialogueTree;

    // Track whether this NPC has been spoken to
    private bool hasSpokenBefore = false;

    // Check if this NPC uses branching dialogue
    public bool UsesBranchingDialogue()
    {
        return useDialogueTree && dialogueTree != null;
    }

    // Get the dialogue tree
    public DialogueTree GetDialogueTree()
    {
        return dialogueTree;
    }

    // Check if we've spoken to this NPC before
    public bool HasSpokenBefore()
    {
        return hasSpokenBefore;
    }

    // Mark that we've spoken to this NPC
    public void SetSpokenBefore()
    {
        hasSpokenBefore = true;
    }

    // Reset the spoken state (for cutscenes that shouldn't mark as spoken)
    public void ResetSpokenState()
    {
        hasSpokenBefore = false;
    }

    // Get repeat dialogue as array
    public string[] GetRepeatDialogue()
    {
        if (repeat != null && repeat.Count > 0)
        {
            return repeat.ToArray();
        }
        return new string[0];
    }

    // Get dialogue array without marking as spoken (for cutscenes)
    public string[] GetDialogueArray()
    {
        if (HasSpokenBefore() && repeat != null && repeat.Count > 0)
        {
            return repeat.ToArray();
        }
        else if (dialogue != null && dialogue.Count > 0)
        {
            return dialogue.ToArray();
        }
        return new string[0];
    }

    // Get the current dialogue based on spoken state
    public string[] GetCurrentDialogue()
    {
        if (UsesBranchingDialogue())
        {
            Debug.LogWarning("GetCurrentDialogue called on NPC with branching dialogue. Use DialogueManager instead.");
            return new string[0];
        }

        return GetDialogueArray();
    }

    // Force set the spoken state (useful for save/load systems)
    public void SetSpokenState(bool spoken)
    {
        hasSpokenBefore = spoken;
    }

    // Validate the dialogue setup
    private void OnValidate()
    {
        if (useDialogueTree && dialogueTree == null)
        {
            Debug.LogWarning($"NPC '{gameObject.name}' has 'Use Dialogue Tree' enabled but no DialogueTree assigned!");
        }

        if (!useDialogueTree && (dialogue == null || dialogue.Count == 0))
        {
            Debug.LogWarning($"NPC '{gameObject.name}' has no dialogue lines set up!");
        }
    }
}