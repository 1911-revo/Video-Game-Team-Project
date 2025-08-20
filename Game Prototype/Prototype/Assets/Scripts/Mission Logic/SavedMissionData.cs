[System.Serializable]
public class SavedMissionData
{
    public string id;
    public string time;
    public bool complete;
    public int chosenDialogueValue;
    public SavedMissionData(SavedMissionStats stats)
    {
        id = stats.id;
        time = stats.time;
        complete = stats.complete;
        chosenDialogueValue = stats.chosenDialogueValue;
    }
}