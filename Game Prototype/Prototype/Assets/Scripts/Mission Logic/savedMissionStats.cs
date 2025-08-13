using UnityEngine;

[CreateAssetMenu(fileName = "SavedMissionStats", menuName = "Saved Mission Stats")]
public class SavedMissionStats : ScriptableObject
{
    public string id; 
    public string time;
    public bool complete;
}
