using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MissionLoader : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform scrollViewContent;

    // Defines a way to add missions to our list
    [Serializable]
    public class MissionInfo
    {
        public string sceneName;
        public string missionText;
    }

    // Array of mission info (scene + display text)
    [SerializeField] private MissionInfo[] missions;

    private void Start()
    {
        PopulateScrollView();
    }
    /// <summary>
    /// Adds the mission buttons to the scrollable view
    /// </summary>
    private void PopulateScrollView()
    {
        foreach (MissionInfo mission in missions)
        {
            // Instantiate button prefab
            GameObject buttonObj = Instantiate(buttonPrefab, scrollViewContent);

            // Get the button component
            Button button = buttonObj.GetComponent<Button>();

            // Set button text to mission text
            TMPro.TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (buttonText != null)
                buttonText.text = mission.missionText;

            // Add onClick event to load the scene
            button.onClick.AddListener(() => LoadScene(mission.sceneName));
        }
    }

    //Loads the requested scene
    public void LoadScene(string sceneName)
    {
        Debug.Log("Loading scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}