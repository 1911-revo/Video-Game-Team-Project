using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// This is the logic for the first mission. Triggers should be called here for easy management.
/// </summary>

public class Mission1Logic : MonoBehaviour
{
    [SerializeField] public GameObject endTrigger;
    [SerializeField] public InventorySystem inventorySystem;

    [SerializeField] public string key;

    public void LeaveLevel()
    {
        if (inventorySystem.collectedItems.Any(item => item.id == key)){
            Debug.Log("Player has key and can leave level");
            SceneManager.LoadSceneAsync("Completion"); //Eventually should be updated for proper async loading.
        }
        else
        {
            Debug.Log("Player doesn't have key, and cannot leave level");
        }
    }
}
