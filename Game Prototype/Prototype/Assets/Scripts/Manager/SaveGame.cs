using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class SaveGame
{
    public static string savePath => Path.Combine(Application.persistentDataPath, "SAVE.SAV");

    private class Wrapper
    {
        public List<SavedMissionData> savedStats = new List<SavedMissionData>();
    }

    private static async Task<Wrapper> LoadWrapperAsync()
    {
        if (!File.Exists(savePath))
        {
            return new Wrapper();
        }
        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<Wrapper>(json) ?? new Wrapper();
    }

    public static async Task Missions(SavedMissionStats stats)
    {
        Wrapper wrapper = await LoadWrapperAsync();
        bool alreadyExists = wrapper.savedStats.Exists(x => x.id == stats.id);
        if (alreadyExists)
        {
            Debug.Log($"Skipped saving {stats.id} as it already exists.");
            return;
        }

        wrapper.savedStats.Add(new SavedMissionData(stats));

        string json = JsonUtility.ToJson(wrapper);
        File.WriteAllText(savePath, json);
        Debug.Log($"Saved {stats.id} to {savePath}");
    }
}