using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class InventorySystem : MonoBehaviour
{
    public item item;
    public List<item> collectedItems = new List<item>();
    public List<GameObject> keyItemObjects = new List<GameObject>();

    [Header("UI References")]
    public Transform inventoryPanel;           // 背包 UI 的父容器（例如一个 Vertical Layout Group）
    public GameObject inventoryTextPrefab;     // 包含 TextMeshProUGUI 的预制体（一个文字条）

    /// <summary>
    /// All key items should be added to this list from the unity editor.
    /// Adds a grey dummy item to the inventory without adding it to the list of collected items.
    /// </summary>
    private void Start()
    {
        foreach (GameObject keyItemObj in keyItemObjects)
        {
            collectable collectable = keyItemObj.GetComponent<collectable>();
            if (collectable != null)
            {
                // Create UI entries for each key item
                GameObject textEntry = Instantiate(inventoryTextPrefab, inventoryPanel);
                var textComponent = textEntry.GetComponentInChildren<TextMeshProUGUI>();
                textComponent.color = Color.gray;
                textComponent.text = collectable.thisItem.name;
            }
        }
    }


    /// <summary>
    /// 将物品名称加入背包并在 UI 中显示。
    /// </summary>
    public void AddItem(item collectedItem)
    {
        collectedItems.Add(collectedItem);
        Debug.Log("Added to inventory: " + collectedItem.name);

        if (inventoryPanel != null && inventoryTextPrefab != null && collectedItem.isKeyItem == false)
        {
            GameObject textEntry = Instantiate(inventoryTextPrefab, inventoryPanel);

            var textComponent = textEntry.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = name;
            }
            else
            {
                Debug.LogWarning("TextMeshProUGUI component not found in instantiated prefab.");
            }
        }
        else if (inventoryPanel != null && inventoryTextPrefab != null && collectedItem.isKeyItem == true)
        {
           var textComponent = findItem(collectedItem);
           textComponent.color = Color.white;
        }
    }

    /// <summary>
    /// Finds a TextMeshPro item based on the display name.
    /// </summary>
    /// <param name="item">The item to find thats being displayed.</param>
    /// <returns></returns>
    private TextMeshProUGUI findItem(item item)
    {
        TextMeshProUGUI[] listedItems = GetComponentsInChildren<TextMeshProUGUI>();
        for (int i = listedItems.Length; i > 0; i--)
        {
            if (listedItems[i - 1].text == item.name)
            {
                return listedItems[i - 1];
            }
        }
        return null;
    }
}
