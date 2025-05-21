using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MissionLoader : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform scrollViewContent;
    [SerializeField] private MissionData missionData;

    // Array of mission info (scene + display text)
    [SerializeField] private MissionData[] missions;

    private void Start()
    {
        PopulateScrollView();
    }
    /// <summary>
    /// Adds the mission buttons to the scrollable view
    /// </summary>
    private void PopulateScrollView()
    {
        foreach (MissionData mission in missions)
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
            button.onClick.AddListener(() => LoadScene(mission.sceneName, mission.missionText, mission.lore, mission.sprite));
        }
    }

    //Loads the requested scene
    public void LoadScene(string sceneName, string missionText, string missionLore, Sprite missionSprite)
    {
        Debug.Log("Loading mission lore screen");
        SceneManager.LoadScene("missionLore");

        // Passes important info to the mission
        missionData.sceneName = sceneName;
        missionData.missionText = missionText;
        missionData.sprite = missionSprite;
        missionData.lore = missionLore;
    }
}