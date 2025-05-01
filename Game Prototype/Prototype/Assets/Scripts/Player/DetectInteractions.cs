using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public class DetectInteractions : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 2f;
    [SerializeField] private LayerMask collectableLayer;

    [Header("Prompt UI")]
    [SerializeField] private GameObject promptPrefab;

    [SerializeField] private InventorySystem inventorySystem;


    private GameObject nearbyCollectable;
    private GameObject activePrompt;

    /// <summary>
    /// Check for interactions with collectables/interactables
    /// </summary>
    void Update()
    {
        // Check for nearby collectables
        CheckForInteractions();

        // Handle input for collection
        if (nearbyCollectable != null && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Interacted with: " + nearbyCollectable.name);
            CollectItem(nearbyCollectable);
        }
    }

    /// <summary>
    /// Find all collectable objects within the detection radius
    /// </summary>
    private void CheckForInteractions()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, collectableLayer);

        // If no collectables were found and we have an active prompt, destroy it
        if (hitColliders.Length == 0)
        {
            if (activePrompt != null)
            {
                Destroy(activePrompt);
                activePrompt = null;
            }
            nearbyCollectable = null;
            return;
        }

        // Find the closest interactable object
        float closestDistance = float.MaxValue;
        GameObject closestCollectable = null;

        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Collectable"))
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCollectable = collider.gameObject;
                }
            }
        }
        // If found a new collectable that's different from the previous one
        if (closestCollectable != null && closestCollectable != nearbyCollectable)
        {
            // Destroy any existing prompt
            if (activePrompt != null)
            {
                Destroy(activePrompt);
            }

            // Create a new prompt above the collectable
            nearbyCollectable = closestCollectable;
            CreatePromptAboveObject(nearbyCollectable);
        }
        else if (closestCollectable == null && activePrompt != null)
        {
            // No collectable found, destroy the prompt
            Destroy(activePrompt);
            activePrompt = null;
            nearbyCollectable = null;
        }
    }

    /// <summary>
    /// Creates text above the object thats interactable.
    /// </summary>
    /// <param name="obj"> The object to create text above </param>
    private void CreatePromptAboveObject(GameObject obj)
    {
        if (promptPrefab != null)
        {
            // Instantiate the prompt as a child of the collectable object
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

    /// <summary>
    /// Deletes an item and currently prints it's in-unity name to the console.
    /// </summary>
    private void CollectItem(GameObject item)
    {
        if (item != null)
        {
            string itemName = item.name;

            Debug.Log("Collected: " + itemName);

            // Add item name to inventory
            if (inventorySystem != null)
            {
                inventorySystem.AddItem(itemName);
            }
            else
            {
                Debug.LogWarning("Inventory system reference is missing!");
            }

            // Destroy prompt and object
            if (activePrompt != null)
            {
                Destroy(activePrompt);
                activePrompt = null;
            }

            Destroy(item);
            nearbyCollectable = null;
        }
        else
        {
            Debug.LogWarning("Tried to collect null item!");
        }
    }
}
