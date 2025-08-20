using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CompletionScreen : MonoBehaviour
{
    [SerializeField] SavedMissionStats stats;

    [SerializeField] TextMeshProUGUI timestamp;

    [SerializeField] private float typingSpeed; //Lower is faster

    [SerializeField] Button playButton;
    [SerializeField] TextMeshProUGUI buttonText;

    private bool isTyping = false;
    bool loadNextScene = false;
    bool gamesaved = false;

    /// <summary>
    /// Sets all UI elements to the required values.
    /// Starts Coroutines for typing out the timestamp, and loading the next scene in the background.    
    /// </summary>
    private void Start()
    {
        timestamp.text = stats.time;
        timestamp.maxVisibleCharacters = 0;

        isTyping = true;
        SaveProgress();
        StartCoroutine(TypeText(stats.time));
        StartCoroutine(PreloadScene("MissionSelect"));
    }

    /// <summary>
    /// Types out text character by character.
    /// </summary>
    /// <param name="desc">The timestamp</param>
    /// <returns></returns>
    IEnumerator TypeText(string desc)
    {
        while ((timestamp.maxVisibleCharacters) < desc.Length)
        {
            timestamp.maxVisibleCharacters++;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        Debug.Log("Typing description complete!");
    }

    /// <summary>
    /// Loads the next scene in the background while the player reads.
    /// </summary>
    /// <param name="sceneName">Scene to load</param>
    /// <returns></returns>
    IEnumerator PreloadScene(string sceneName)
    {
        yield return null;

        Debug.Log(sceneName);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        Debug.Log("Pro :" + asyncOperation.progress);
        while (!asyncOperation.isDone)
        {
            //Output the current progress
            buttonText.text = "Loading videos: " + (asyncOperation.progress * 100) + "%";

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f && gamesaved == true)
            {
                //Change the Text to show the Level is ready
                buttonText.text = "Return";
                //Wait to you press the button to load the level
                if (loadNextScene)
                {
                    asyncOperation.allowSceneActivation = true;
                }

            }

            yield return null;
        }

    }

    public async void SaveProgress()
    {
        stats.id = PlayerPrefs.GetString("CurrentMissionId");
        stats.complete = true;

        await SaveGame.Missions(stats);
        gamesaved = true;
    }


    /// <summary>
    /// Displays all the mission lore when the player presses enter.
    /// </summary>
    private void Update()
    {
        if (isTyping && Input.GetKeyDown(KeyCode.Return))
        {
            StopCoroutine(TypeText(stats.time));
            timestamp.maxVisibleCharacters = int.MaxValue;
            isTyping = false;
        }
    }

    /// <summary>
    /// Called by the playbutton for the preload coroutine.
    /// </summary>
    public void buttonClick()
    {
        loadNextScene = true;
    }
}
