using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEditor.Overlays;
using UnityEngine;

public static class SaveGame
{
    public static string savePath => Path.Combine(Application.persistentDataPath, "SAVE.SAV");

    private class Wrapper
    {
        public List<SavedMissionStats> savedStats = new List<SavedMissionStats>(); 
    }

    private static Wrapper LoadWrapper()
    {
        if (!File.Exists(savePath))
        {
            return new Wrapper();
        }

        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<Wrapper>(json) ?? new Wrapper();
    }

    public static void Save(SavedMissionStats stats)
    {
        Wrapper wrapper = LoadWrapper();

        bool alreadyExists = wrapper.savedStats.Exists(x => x.id == stats.id);
        if (alreadyExists)
        {
            Debug.Log($"Skipped saving {stats.name} as it already exists.");
            return;
        }

        wrapper.savedStats.Add(new SavedMissionStats
        {
            id = stats.id,
            time = stats.time,
            complete = stats.complete
        });

        string json = JsonUtility.ToJson(wrapper.savedStats);
        File.WriteAllText(savePath, json);
        Debug.Log($"Saved {stats.name} to {savePath}");
    }
}
