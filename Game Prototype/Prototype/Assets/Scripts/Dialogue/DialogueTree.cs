using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Tree")]
public class DialogueTree : ScriptableObject
{
    [SerializeField] public List<DialogueNode> nodes = new List<DialogueNode>();
    [SerializeField] public string startNodeID;
    
    // Audio storage
    [Header("Audio")]
    [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();
    [SerializeField] private List<string> audioClipNames = new List<string>();
    
    // Helper to find a node by ID
    public DialogueNode GetNodeByID(string id)
    {
        return nodes.Find(node => node.nodeID == id);
    }
    
    // Get starting node
    public DialogueNode GetStartNode()
    {
        return GetNodeByID(startNodeID);
    }
    
    // Create a dictionary to store audio
    public Dictionary<string, AudioClip> GetAudioDictionary()
    {
        Dictionary<string, AudioClip> soundDictionary = new Dictionary<string, AudioClip>();
        
        for (int i = 0; i < Mathf.Min(audioClips.Count, audioClipNames.Count); i++)
        {
            if (audioClips[i] != null && !string.IsNullOrEmpty(audioClipNames[i]))
            {
                soundDictionary[audioClipNames[i]] = audioClips[i];
            }
        }
        
        return soundDictionary;
    }
    
    // Gets audio clip by name
    public AudioClip GetAudioClip(string clipName)
    {
        int index = audioClipNames.IndexOf(clipName);
        if (index >= 0 && index < audioClips.Count)
        {
            return audioClips[index];
        }
        return null;
    }
}