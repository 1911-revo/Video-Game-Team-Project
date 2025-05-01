using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventorySystem : MonoBehaviour
{
    // 存储所有收集到的物品名称
    public List<string> collectedItems = new List<string>();

    [Header("UI References")]
    public Transform inventoryPanel;           // 背包 UI 的父容器（例如一个 Vertical Layout Group）
    public GameObject inventoryTextPrefab;     // 包含 TextMeshProUGUI 的预制体（一个文字条）

    /// <summary>
    /// 将物品名称加入背包并在 UI 中显示。
    /// </summary>
    public void AddItem(string itemName)
    {
        collectedItems.Add(itemName);
        Debug.Log("Added to inventory: " + itemName);

        if (inventoryPanel != null && inventoryTextPrefab != null)
        {
            GameObject textEntry = Instantiate(inventoryTextPrefab, inventoryPanel);

            var textComponent = textEntry.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = itemName;
            }
            else
            {
                Debug.LogWarning("TextMeshProUGUI component not found in instantiated prefab.");
            }
        }

    }
}
