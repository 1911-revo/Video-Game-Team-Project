using UnityEngine;

[CreateAssetMenu(fileName = "MissionData", menuName = "Mission Data")]
public class MissionData : ScriptableObject
{
    public string sceneName;
    public string missionText;
    public string lore;
    public Sprite sprite;

    //This is where we can import weapons into individual levels.
}