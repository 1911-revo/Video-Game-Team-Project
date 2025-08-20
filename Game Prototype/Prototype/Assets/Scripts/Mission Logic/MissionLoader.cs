using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class MissionLoader : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform scrollViewContent;
    [SerializeField] private MissionData missionData;
    [SerializeField] private MissionData[] missions;

    private List<SavedMissionData> completedMissions = new List<SavedMissionData>();

    private async void Start()
    {
        await LoadSaveData();
        PopulateScrollView();
    }

    /// <summary>
    /// Load the saved mission data
    /// </summary>
    private async Task LoadSaveData()
    {
        try
        {
            string savePath = Path.Combine(Application.persistentDataPath, "SAVE.SAV");
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                var wrapper = JsonUtility.FromJson<SaveWrapper>(json);
                if (wrapper != null)
                {
                    completedMissions = wrapper.savedStats;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading save data: {e.Message}");
        }
    }

    /// <summary>
    /// Check if a mission is completed
    /// </summary>
    private bool IsMissionCompleted(string missionId)
    {
        return completedMissions.Exists(m => m.id == missionId && m.complete);
    }

    /// <summary>
    /// Check if all prerequisites for a mission are met
    /// </summary>
    private bool ArePrerequisitesMet(MissionData mission)
    {
        if (mission.prerequisites == null || mission.prerequisites.Length == 0)
            return true; // No prerequisites means it's always available

        foreach (MissionData prerequisite in mission.prerequisites)
        {
            if (!IsMissionCompleted(prerequisite.missionId))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Adds the mission buttons to the scrollable view
    /// </summary>
    private void PopulateScrollView()
    {
        foreach (MissionData mission in missions)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, scrollViewContent);
            Button button = buttonObj.GetComponent<Button>();
            TMPro.TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();

            bool isCompleted = IsMissionCompleted(mission.missionId);
            bool prerequisitesMet = ArePrerequisitesMet(mission);

            // Set button text
            if (buttonText != null)
            {
                if (isCompleted)
                {
                    buttonText.text = mission.missionText + " [COMPLETED]";
                    buttonText.color = Color.green;
                }
                else if (!prerequisitesMet)
                {
                    buttonText.text = mission.missionText + " [LOCKED]";
                    buttonText.color = Color.gray;
                }
                else
                {
                    buttonText.text = mission.missionText;
                    buttonText.color = Color.white;
                }
            }

            // Set button interactability
            button.interactable = prerequisitesMet && !isCompleted;

            // Only add click listener if mission is playable
            if (prerequisitesMet && !isCompleted)
            {
                button.onClick.AddListener(() => LoadScene(mission.missionId, mission.sceneName, mission.missionText, mission.lore, mission.sprite));
            }
        }
    }

    public void LoadScene(string id, string sceneName, string missionText, string missionLore, Sprite missionSprite)
    {
        SceneManager.LoadScene("missionLore");

        PlayerPrefs.SetString("CurrentMissionId", id); // Really bodgy fix but we're running out of time and I dont have time to fix it :sob:
        missionData.sceneName = sceneName;
        missionData.missionText = missionText;
        missionData.sprite = missionSprite;
        missionData.lore = missionLore;
    }
}

[System.Serializable]
public class SaveWrapper
{
    public List<SavedMissionData> savedStats = new List<SavedMissionData>();
}