using TMPro;
using UnityEngine;

public class updateMissionStats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timestamp;
    [SerializeField] public SavedMissionStats stats;

    float timer = 0f;

    /// <summary>
    /// Keeps track of time spent in level
    /// </summary>
    void Update()
    {
        timer += Time.deltaTime;
        int hours = Mathf.FloorToInt(timer / 3600f);
        int minutes = Mathf.FloorToInt((timer % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);
        int fakeframes = Mathf.FloorToInt((timer * 30) % 30);

        string time = string.Format("{0:00}:{1:00}:{2:00}:{3:00}", hours, minutes, seconds, fakeframes);

        timestamp.text = time;

        stats.time = time;
    }
}
