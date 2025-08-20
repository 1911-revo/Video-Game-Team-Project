using UnityEngine;

[CreateAssetMenu(fileName = "MissionData", menuName = "Mission Data")]
public class MissionData : ScriptableObject
{
    [Header("Mission Info")]
    public string missionId;
    public string sceneName;
    public string missionText;
    public string lore;
    public Sprite sprite;

    [Header("Prerequisites")]
    public MissionData[] prerequisites;
}