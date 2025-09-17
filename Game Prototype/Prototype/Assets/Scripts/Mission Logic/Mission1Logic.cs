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

    [SerializeField] public AudioSource[] audioSource;

    private void Start()
    {
        DialogueManager.ResetMissionDialogueScore();
    }

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

    public void ChangeMusic()
    {
        if (audioSource[0].isPlaying)
        {
            audioSource[0].Stop();
            audioSource[1].Play();
        }
    }
}
