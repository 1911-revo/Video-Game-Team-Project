using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class DetectInteractions : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 2f;
    [SerializeField] private LayerMask collectableLayer;
    [SerializeField] private LayerMask npcLayer;

    [Header("Prompt UI")]
    [SerializeField] private GameObject promptPrefab;

    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private Dialogue dialogueManager;

    private GameObject nearbyCollectable;
    private GameObject nearbyNPC;
    private GameObject activePrompt;

    // Track NPCs that have been talked to
    private HashSet<string> interactedNPCs = new HashSet<string>();

    // Track which object is currently prioritized for interaction
    private enum InteractionType { None, Collectable, NPC }
    private InteractionType currentInteraction = InteractionType.None;

    void Update()
    {
        // Check for nearby interactables
        CheckForInteractions();

        // Handle input for interactions
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Determine which interaction to prioritize
            switch (currentInteraction)
            {
                case InteractionType.Collectable:
                    if (nearbyCollectable != null)
                    {
                        CollectItem(nearbyCollectable);
                    }
                    break;

                case InteractionType.NPC:
                    if (nearbyNPC != null)
                    {
                        TalkToNPC(nearbyNPC);
                    }
                    break;
            }
        }
    }

    private void CheckForInteractions()
    {
        // Find all collectables and NPCs within the detection radius
        Collider2D[] collectableColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, collectableLayer);
        Collider2D[] npcColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, npcLayer);

        // If no interactables were found, clean up and return
        if (collectableColliders.Length == 0 && npcColliders.Length == 0)
        {
            CleanupPrompt();
            return;
        }

        // Find the closest interactable object
        float closestCollectableDistance = float.MaxValue;
        GameObject closestCollectable = null;

        float closestNPCDistance = float.MaxValue;
        GameObject closestNPC = null;

        // Find closest collectable
        foreach (Collider2D collider in collectableColliders)
        {
            if (collider.CompareTag("Collectable"))
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < closestCollectableDistance)
                {
                    closestCollectableDistance = distance;
                    closestCollectable = collider.gameObject;
                }
            }
        }

        // Find closest NPC
        foreach (Collider2D collider in npcColliders)
        {
            if (collider.CompareTag("NPC"))
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < closestNPCDistance)
                {
                    closestNPCDistance = distance;
                    closestNPC = collider.gameObject;
                }
            }
        }

        // Determine which is closer: the closest collectable or the closest NPC
        if (closestNPC != null && (closestCollectable == null || closestNPCDistance <= closestCollectableDistance))
        {
            // NPC is closer (or there is no collectable), prioritize NPC interaction
            if (nearbyNPC != closestNPC || currentInteraction != InteractionType.NPC)
            {
                CleanupPrompt();
                nearbyNPC = closestNPC;
                nearbyCollectable = null;
                currentInteraction = InteractionType.NPC;
                CreatePromptAboveObject(nearbyNPC);
            }
        }
        else if (closestCollectable != null)
        {
            // Collectable is closer (or there is no NPC), prioritize collectable interaction
            if (nearbyCollectable != closestCollectable || currentInteraction != InteractionType.Collectable)
            {
                CleanupPrompt();
                nearbyCollectable = closestCollectable;
                nearbyNPC = null;
                currentInteraction = InteractionType.Collectable;
                CreatePromptAboveObject(nearbyCollectable);
            }
        }
        else
        {
            // No interactables found, clean up
            CleanupPrompt();
        }
    }

    private void CleanupPrompt()
    {
        if (activePrompt != null)
        {
            Destroy(activePrompt);
            activePrompt = null;
        }
        nearbyCollectable = null;
        nearbyNPC = null;
        currentInteraction = InteractionType.None;
    }

    private void CreatePromptAboveObject(GameObject obj)
    {
        if (promptPrefab != null)
        {
            // Instantiate the prompt as a child of the interactable object
            activePrompt = Instantiate(promptPrefab, obj.transform);
            activePrompt.transform.localPosition = new Vector3(0, 1, 0);

            // Get the non-UI TextMeshPro component attached to the prompt
            TextMeshPro tmp = activePrompt.GetComponentInChildren<TextMeshPro>();
            if (tmp != null)
            {
                tmp.text = "E";
            }
            else
            {
                Debug.LogWarning("No TextMeshPro component found on the prompt prefab.");
            }
        }
    }

    private void CollectItem(GameObject item)
    {
        if (item != null)
        {
            collectable collectableScript = item.GetComponent<collectable>();
            item collectedItem = collectableScript.thisItem;
            Debug.Log("Collected: " + collectedItem);

            // Add item name to inventory
            if (inventorySystem != null)
            {
                inventorySystem.AddItem(collectedItem);
            }
            else
            {
                Debug.LogWarning("Inventory system reference is missing!");
            }

            CleanupPrompt();
            Destroy(item);
        }
        else
        {
            Debug.LogWarning("Tried to collect null item!");
        }
    }

    private void TalkToNPC(GameObject npc)
    {
        if (npc != null && dialogueManager != null)
        {
            string npcID = npc.name; // Using the NPC's name as a unique identifier
            NPCDialogue npcDialogue = npc.GetComponent<NPCDialogue>();

            if (npcDialogue != null)
            {
                // Determine if we should use regular or repeat dialogue
                string[] dialogueToUse;

                if (interactedNPCs.Contains(npcID) && npcDialogue.repeat.Count > 0)
                {
                    // Convert List<string> to string[]
                    dialogueToUse = npcDialogue.repeat.ToArray();
                    Debug.Log("Using repeat dialogue for: " + npcID);
                }
                else
                {
                    // First-time dialogue
                    dialogueToUse = npcDialogue.dialogue.ToArray();
                    interactedNPCs.Add(npcID); // Mark as interacted
                    Debug.Log("Using first-time dialogue for: " + npcID);
                }

                // Start dialogue
                dialogueManager.StartDialogue(dialogueToUse);

                // Hide the prompt while in dialogue
                if (activePrompt != null)
                {
                    activePrompt.SetActive(false);
                }
            }
            else
            {
                Debug.LogWarning("NPC " + npcID + " does not have an NPCDialogue component!");
            }
        }
        else
        {
            Debug.LogWarning("Either NPC is null or dialogue manager reference is missing!");
        }
    }
}