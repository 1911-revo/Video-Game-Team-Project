using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Throbber : MonoBehaviour
{
    [SerializeField] MissionData selectedMission;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.LoadSceneAsync(selectedMission.sceneName, LoadSceneMode.Single);
    }
}
