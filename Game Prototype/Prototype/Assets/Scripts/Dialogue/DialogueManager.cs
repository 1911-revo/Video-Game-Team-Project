using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Dialogue dialogueController;
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private PlayerController playerController;

    [Header("Optional")]
    [SerializeField] private float delayAfterTyping = 0.2f;

    private DialogueTree currentTree;
    private DialogueNode currentNode;
    private bool dialogueActive = false;
    private NPCDialogue currentNPC;

    // Track last selected choice for branching cutscenes
    private string lastSelectedNodeID = "";

    void Start()
    {
        // Validate required components
        if (dialogueController == null)
        {
            Debug.LogError("Dialogue controller reference is missing! Please assign it in the inspector.");
        }

        if (choicePanel == null)
        {
            Debug.LogError("Choice panel reference is missing! Please assign it in the inspector.");
        }

        if (choiceButtonPrefab == null)
        {
            Debug.LogError("Choice button prefab reference is missing! Please assign it in the inspector.");
        }

        // Find player controller if not assigned
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
            if (playerController == null)
            {
                Debug.LogWarning("PlayerController not found! Player movement will not be restricted during dialogue.");
            }
        }

        // Hide choice panel at start
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);

            // Make sure the choice panel is properly positioned
            RectTransform panelRect = choicePanel.GetComponent<RectTransform>();
            if (panelRect != null)
            {
                panelRect.anchorMin = new Vector2(0.5f, 0.5f);
                panelRect.anchorMax = new Vector2(0.5f, 0.5f);
                panelRect.pivot = new Vector2(0.5f, 0.5f);

                // Position it at the center or slightly above center
                panelRect.anchoredPosition = new Vector2(0, -20);

                // Give it a reasonable size
                panelRect.sizeDelta = new Vector2(400, 60);
            }
        }

        // Set up the choice panel layout for horizontal display
        EnsureChoicePanelHasLayout();

        // Initialize state
        dialogueActive = false;
        currentTree = null;
        currentNode = null;
        currentNPC = null;
        lastSelectedNodeID = "";
    }

    // Start dialogue from a specific node (for cutscenes)
    public void StartDialogueFromNode(NPCDialogue npc, string nodeID)
    {
        if (npc == null || string.IsNullOrEmpty(nodeID))
        {
            Debug.LogError("Invalid parameters for StartDialogueFromNode");
            return;
        }

        currentNPC = npc;

        if (npc.UsesBranchingDialogue() && npc.GetDialogueTree() != null)
        {
            DisablePlayerMovement();

            currentTree = npc.GetDialogueTree();
            currentNode = currentTree.GetNodeByID(nodeID);

            if (currentNode == null)
            {
                Debug.LogError($"Node '{nodeID}' not found in dialogue tree!");
                return;
            }

            dialogueActive = true;
            ShowCurrentNode();
        }
    }

    // Start dialogue with just a DialogueTree (for cutscenes)
    public void StartDialogueTree(DialogueTree tree, string startNodeID = "")
    {
        if (tree == null)
        {
            Debug.LogError("DialogueTree is null!");
            return;
        }

        // Disable player movement
        DisablePlayerMovement();

        currentTree = tree;
        currentNPC = null; // No NPC associated with this dialogue

        // Get the appropriate start node
        if (!string.IsNullOrEmpty(startNodeID))
        {
            currentNode = tree.GetNodeByID(startNodeID);
        }
        else
        {
            currentNode = tree.GetStartNode(false); // Always use first-time dialogue for cutscenes
        }

        if (currentNode == null)
        {
            Debug.LogError("Start node not found in dialogue tree!");
            return;
        }

        dialogueActive = true;
        ShowCurrentNode();
    }

    // Start a new dialogue with an NPC
    public void StartDialogue(NPCDialogue npc)
    {
        if (npc == null)
        {
            Debug.LogError("NPC is null :(");
            return;
        }

        currentNPC = npc;

        // Check if this NPC uses branching dialogue
        if (npc.UsesBranchingDialogue())
        {
            StartBranchingDialogue(npc);
        }
        else
        {
            StartLinearDialogue(npc);
        }
    }

    // Start branching dialogue with dialogue tree
    private void StartBranchingDialogue(NPCDialogue npc)
    {
        DialogueTree tree = npc.GetDialogueTree();
        if (tree == null)
        {
            Debug.LogError("DialogueTree is null for NPC: " + npc.name);
            return;
        }

        // Disable player movement when dialogue starts
        DisablePlayerMovement();

        currentTree = tree;

        // Get the appropriate start node based on whether we've spoken before
        bool hasSpokenBefore = npc.HasSpokenBefore();
        currentNode = currentTree.GetStartNode(hasSpokenBefore);

        if (currentNode == null)
        {
            Debug.LogError("Start node not found in dialogue tree!");
            return;
        }

        dialogueActive = true;
        ShowCurrentNode();

        // Mark that we've spoken to this NPC
        npc.SetSpokenBefore();
    }

    // Start linear dialogue (old system with dialogue/repeat lists)
    private void StartLinearDialogue(NPCDialogue npc)
    {
        string[] dialogueToUse;

        if (npc.HasSpokenBefore() && npc.repeat != null && npc.repeat.Count > 0)
        {
            // Use repeat dialogue
            dialogueToUse = npc.GetRepeatDialogue();
        }
        else if (npc.dialogue != null && npc.dialogue.Count > 0)
        {
            // Use first-time dialogue
            dialogueToUse = npc.dialogue.ToArray();
        }
        else
        {
            Debug.LogError("No dialogue found for NPC: " + npc.name);
            return;
        }

        // Disable player movement when dialogue starts
        DisablePlayerMovement();

        dialogueActive = true;

        // Start the dialogue without sounds (old system doesn't have DialogueTree sounds)
        dialogueController.StartDialogue(dialogueToUse);

        // Mark that we've spoken to this NPC
        npc.SetSpokenBefore();

        // Set up coroutine to wait for dialogue completion
        StartCoroutine(WaitForLinearDialogueCompletion());
    }

    // Wait for linear dialogue to complete
    private IEnumerator WaitForLinearDialogueCompletion()
    {
        // Wait until dialogue is complete
        while (dialogueController.dialogueStarted)
        {
            yield return null;
        }

        // End dialogue
        EndDialogue();
    }

    // Display the current dialogue node - FIXED VERSION
    private void ShowCurrentNode()
    {
        if (currentNode == null)
        {
            Debug.LogError("Current node is null!");
            return;
        }

        // Get the sound dictionary from the current tree
        Dictionary<string, AudioClip> sounds = null;
        if (currentTree != null)
        {
            sounds = currentTree.GetAudioDictionary();
        }

        // FIXED: Use single line dialogue WITHOUT callback for individual nodes
        // Only the final node should trigger the completion callback
        dialogueController.StartDialogue(currentNode.text, null, sounds);

        // Handle node completion ourselves
        StartCoroutine(HandleNodeCompletion());
    }

    // Handle what happens after a node's text finishes displaying
    private IEnumerator HandleNodeCompletion()
    {
        // Wait until the current node finishes typing
        while (dialogueController.isTyping)
        {
            yield return null;
        }

        // Wait a short moment
        yield return new WaitForSeconds(delayAfterTyping);

        // Show choices or continue to next node
        if (currentNode.choices != null && currentNode.choices.Count > 0)
        {
            // Show choice buttons
            ShowChoices();
        }
        else if (!string.IsNullOrEmpty(currentNode.nextNodeID))
        {
            // No choices - just wait for Enter key
            dialogueController.WaitForChoice(false);

            // Wait for player to press Enter
            yield return StartCoroutine(WaitForEnterInput());

            // Player pressed Enter, move to next node
            currentNode = currentTree.GetNodeByID(currentNode.nextNodeID);
            if (currentNode != null)
            {
                ShowCurrentNode(); // Show the next node
            }
            else
            {
                // End the entire dialogue tree - THIS is when we signal completion
                EndDialogue();
            }
        }
        else
        {
            // End of dialogue branch - wait for Enter, then end the whole tree
            dialogueController.WaitForChoice(false);
            yield return StartCoroutine(WaitForEnterInput());

            // THIS is when the entire dialogue tree is complete
            EndDialogue();
        }
    }

    private IEnumerator WaitForEnterInput()
    {
        // Make sure we don't catch the same keypress twice
        yield return null;

        // Wait for player to press Enter
        while (!Input.GetKeyDown(KeyCode.Return) && !Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            yield return null;
        }
    }

    // Show dialogue choices
    private void ShowChoices()
    {
        if (choicePanel == null || choiceButtonPrefab == null) return;

        // Clear existing choices
        foreach (Transform child in choicePanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Ensure layout is set up correctly for horizontal arrangement
        EnsureChoicePanelHasLayout();

        // Create button for each choice
        if (currentNode.choices != null && currentNode.choices.Count > 0)
        {
            foreach (var choice in currentNode.choices)
            {
                GameObject buttonObj = Instantiate(choiceButtonPrefab, choicePanel.transform);
                Button button = buttonObj.GetComponent<Button>();

                // Disable keyboard navigation
                Navigation nav = new Navigation();
                nav.mode = Navigation.Mode.None;
                button.navigation = nav;

                // Configure button for horizontal layout
                RectTransform rt = buttonObj.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);

                // Fixed size for consistent side-by-side appearance
                rt.sizeDelta = new Vector2(160, 40);

                // Add or update layout element for horizontal sizing
                LayoutElement layoutElement = buttonObj.GetComponent<LayoutElement>();
                if (layoutElement == null)
                {
                    layoutElement = buttonObj.AddComponent<LayoutElement>();
                }
                layoutElement.minWidth = 160;
                layoutElement.preferredWidth = 160;
                layoutElement.minHeight = 40;
                layoutElement.preferredHeight = 40;
                layoutElement.flexibleWidth = 0;

                // Set the button text
                TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = choice.choiceText;
                    buttonText.alignment = TextAlignmentOptions.Center;

                    // For horizontal buttons, make sure text wraps properly
                    buttonText.textWrappingMode = TextWrappingModes.Normal;
                    buttonText.overflowMode = TextOverflowModes.Truncate;
                }

                // Add onClick handler
                string targetNodeID = choice.targetNodeID;
                button.onClick.AddListener(() => OnChoiceSelected(targetNodeID));
            }
        }

        // Position the choice panel properly for a horizontal layout
        RectTransform choicePanelRect = choicePanel.GetComponent<RectTransform>();
        if (choicePanelRect != null)
        {
            choicePanelRect.anchorMin = new Vector2(0.5f, 0.5f);
            choicePanelRect.anchorMax = new Vector2(0.5f, 0.5f);
            choicePanelRect.pivot = new Vector2(0.5f, 0.5f);

            Vector2 currentPosition = choicePanelRect.anchoredPosition;
            choicePanelRect.anchoredPosition = new Vector2(currentPosition.x, 10);
        }

        // Show the choice panel
        choicePanel.SetActive(true);

        // Disable normal dialogue advancement
        dialogueController.WaitForChoice(true);

        // Clear selection
        EventSystem.current.SetSelectedGameObject(null);

        // Force layout rebuild
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(choicePanelRect);
    }

    // Handle choice selection - UPDATED
    private void OnChoiceSelected(string targetNodeID)
    {
        // Store the selected node ID for cutscene branching
        lastSelectedNodeID = targetNodeID;

        // Notify cutscene manager if it exists
        CutsceneManager cutsceneManager = FindObjectOfType<CutsceneManager>();
        if (cutsceneManager != null)
        {
            cutsceneManager.lastDialogueChoice = targetNodeID;
        }

        // Hide the choice panel
        choicePanel.SetActive(false);

        // Re-enable normal dialogue advancement
        dialogueController.WaitForChoice(false);

        // Move to the selected node
        DialogueNode nextNode = currentTree.GetNodeByID(targetNodeID);
        if (nextNode != null)
        {
            currentNode = nextNode;
            ShowCurrentNode(); // This will properly display the next node
        }
        else
        {
            Debug.LogError("Target node not found: " + targetNodeID);
            EndDialogue();
        }
    }

    // End the dialogue - ONLY called when the entire tree is complete
    public void EndDialogue()
    {
        // IMPORTANT: Only signal completion when the ENTIRE dialogue tree is finished
        dialogueController.EndDialogue(); // This will trigger the cutscene callback

        dialogueActive = false;
        currentTree = null;
        currentNode = null;
        currentNPC = null;
        lastSelectedNodeID = "";

        // Hide choice panel if visible
        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

        // Re-enable player movement when dialogue ends
        EnablePlayerMovement();
    }

    // Disable player movement during dialogue
    private void DisablePlayerMovement()
    {
        if (playerController != null)
        {
            playerController.SetMovementEnabled(false);
        }
    }

    // Enable player movement after dialogue
    private void EnablePlayerMovement()
    {
        if (playerController != null)
        {
            playerController.SetMovementEnabled(true);
        }
    }

    // Beating the choice buttons into submission
    private void EnsureChoicePanelHasLayout()
    {
        if (choicePanel == null) return;

        // Remove any existing components that might conflict
        ContentSizeFitter existingSizeFitter = choicePanel.GetComponent<ContentSizeFitter>();
        if (existingSizeFitter != null)
        {
            DestroyImmediate(existingSizeFitter);
        }

        // Remove vertical layout if it exists
        VerticalLayoutGroup existingVertical = choicePanel.GetComponent<VerticalLayoutGroup>();
        if (existingVertical != null)
        {
            DestroyImmediate(existingVertical);
        }

        // Remove horizontal layout if it exists
        HorizontalLayoutGroup existingHorizontal = choicePanel.GetComponent<HorizontalLayoutGroup>();
        if (existingHorizontal != null)
        {
            DestroyImmediate(existingHorizontal);
        }

        // Add horizontal layout group for side-by-side buttons
        HorizontalLayoutGroup layoutGroup = choicePanel.AddComponent<HorizontalLayoutGroup>();
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.spacing = 20f;
        layoutGroup.padding = new RectOffset(10, 10, 10, 10);
        layoutGroup.childControlWidth = false;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = false;

        // Add content size fitter to adjust the panel size based on buttons
        ContentSizeFitter sizeFitter = choicePanel.AddComponent<ContentSizeFitter>();
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    public string GetLastSelectedNodeID()
    {
        return lastSelectedNodeID;
    }

    // Check if dialogue is currently active
    public bool IsDialogueActive()
    {
        return dialogueActive;
    }
}