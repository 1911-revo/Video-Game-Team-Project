using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SavedMissionStats", menuName = "Saved Mission Stats")]
[System.Serializable]
public class SavedMissionStats : ScriptableObject
{
    public string id; 
    public string time;
    public bool complete;

    public int chosenDialogueValue;
}
