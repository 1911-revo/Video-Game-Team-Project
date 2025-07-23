using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class missionLoreScreen : MonoBehaviour
{
    [SerializeField] MissionData selectedMission;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] Image image;

    [SerializeField] private float typingSpeed; //Lower is faster

    [SerializeField] Button playButton;
    [SerializeField] TextMeshProUGUI buttonText;

    private bool isTyping = false;
    bool loadNextScene = false;

    /// <summary>
    /// Sets all UI elements to the required values.
    /// Starts Coroutines for typing out the mission text, and loading the next scene in the background.    
    /// </summary>
    private void Start()
    {
        title.text = selectedMission.missionText;
        image.sprite = selectedMission.sprite;
        description.text = selectedMission.lore;
        description.maxVisibleCharacters = 0;

        isTyping = true;
        StartCoroutine(TypeText(selectedMission.lore));
        StartCoroutine(PreloadScene(selectedMission.sceneName));
    }
    
    /// <summary>
    /// Types out text character by character.
    /// </summary>
    /// <param name="desc">The mission background that the player needs to know.</param>
    /// <returns></returns>
    IEnumerator TypeText(string desc)
    {
        while ((description.maxVisibleCharacters) < desc.Length)
        {
            description.maxVisibleCharacters++;
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
            buttonText.text = "Loading progress: " + (asyncOperation.progress * 100) + "%";

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                //Change the Text to show the Level is ready
                buttonText.text = "Play Video";
                //Wait to you press the button to load the level
                if (loadNextScene)
                {
                    asyncOperation.allowSceneActivation = true;
                }

            }

            yield return null;
        }

    }


    /// <summary>
    /// Displays all the mission lore when the player presses enter.
    /// </summary>
    private void Update() {
        if (isTyping && Input.GetKeyDown(KeyCode.Return))
        {
            StopCoroutine(TypeText(selectedMission.lore));
            description.maxVisibleCharacters = int.MaxValue;
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
