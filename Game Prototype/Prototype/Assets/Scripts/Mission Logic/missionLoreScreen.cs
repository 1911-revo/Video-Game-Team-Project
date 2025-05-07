using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class missionLoreScreen : MonoBehaviour
{
    [SerializeField] MissionData selectedMission;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] Image image;

    [SerializeField] private float typingSpeed; //Lower is faster

    private bool isTyping = false;



    private void Start()
    {
        title.text = selectedMission.missionText;
        image.sprite = selectedMission.sprite;
        Debug.Log(selectedMission.lore);
        Debug.Log("test");

        description.text = selectedMission.lore;
        description.maxVisibleCharacters = 0;

        isTyping = true;
        StartCoroutine(TypeText(selectedMission.lore));
    }
    
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

    private void Update() {
        if (isTyping && Input.GetKeyDown(KeyCode.Return))
        {
            StopCoroutine(TypeText(selectedMission.lore));
            description.maxVisibleCharacters = int.MaxValue;
            isTyping = false;
        }
    }
}
