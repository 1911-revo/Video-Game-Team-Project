using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float shakeAmount = 0.5f;
    
    private string[] dialogueLines;
    private int currentLineIndex;
    private bool isTyping;
    private bool dialogueStarted;
    private Coroutine typingCoroutine;
    private Coroutine shakeCoroutine;
    
    // Store indices of characters that should shake
    private List<int> shakeIndices = new List<int>();
    
    string[] testDialogue = new string[]
    {
        "Hello, this is test dialogue.",
        "Some of this text should appear *#FF0000(red).",
        "This text should *SHAKE(shake).",
        "This text should be written, and then *CHANGE((change.),(stay exactly the same)). Woah thats so crazy!",
        "And effects *SHAKE(*#0000FF(Should stack)) (Hopefully)."
    };
    
    void Start()
    {
        if (textComponent == null)
        {
            Debug.LogError("TextMeshProUGUI component not found on the text GameObject!");
        }
        textComponent.text = string.Empty;
        ///TEMP
        StartDialogue(testDialogue);
    }
    
    void Update()
    {
        // Handle Enter key to advance dialogue
        if (dialogueStarted && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            if (isTyping)
            {
                // Still typing, so complete current line
                CompleteCurrentLine();
            }
            else
            {
                // Move to next line
                currentLineIndex++;
                DisplayNextLine();
            }
        }
    }
    
    // Start dialogue with given text lines
    public void StartDialogue(string[] lines)
    {
        StopAllCoroutines();
        
        dialogueLines = lines;
        currentLineIndex = 0;
        dialogueStarted = true;
        
        shakeIndices.Clear();
        
        DisplayNextLine();
    }
    
    // Show next line in the sequence
    public void DisplayNextLine()
    {
        if (!dialogueStarted)
            return;
            
        // Check if we're out of lines
        if (currentLineIndex >= dialogueLines.Length)
        {
            EndDialogue();
            return;
        }
        
        // If mid-typing, complete it immediately
        if (isTyping)
        {
            CompleteCurrentLine();
            return;
        }
        
        shakeIndices.Clear();
        
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }
        
        typingCoroutine = StartCoroutine(TypeLine(dialogueLines[currentLineIndex]));
    }
    
    // Display full line immediately
    public void CompleteCurrentLine()
    {
        if (isTyping && typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
            
            string line = dialogueLines[currentLineIndex];
            
            // For CHANGE command, skip to second part
            Regex changeRegex = new Regex(@"\*CHANGE\(\(([^)]*)\),\(([^)]*)\)\)");
            if (changeRegex.IsMatch(line))
            {
                line = changeRegex.Replace(line, match => match.Groups[2].Value);
            }
            
            // Process the text with all formatting
            string processedText = ProcessFormattingWithShake(line, out List<ShakeRange> shakeRanges);
            textComponent.text = processedText;
            
            textComponent.ForceMeshUpdate();
            
            SetupShakeIndicesFromRanges(shakeRanges);
            
            if (shakeIndices.Count > 0 && shakeCoroutine == null)
            {
                shakeCoroutine = StartCoroutine(ShakeCharacters());
            }
        }
    }
    
    // Simple class to track character ranges that should shake
    private class ShakeRange
    {
        public int startIndex;
        public int length;
        
        public ShakeRange(int start, int length)
        {
            this.startIndex = start;
            this.length = length;
        }
    }
    
    // Process text formatting and collect shake ranges
    private string ProcessFormattingWithShake(string input, out List<ShakeRange> shakeRanges)
    {
        shakeRanges = new List<ShakeRange>();
        string processedText = input;
        
        // Handle nested formatting
        // Process color formatting everywhere including in shake areas
        // *#RRGGBB(text) -> <color=#RRGGBB>text</color>
        string colorPattern = @"\*#([A-Fa-f0-9]{6})\(([^)]*)\)";
        processedText = Regex.Replace(processedText, colorPattern, match => {
            string colorCode = match.Groups[1].Value;
            string text = match.Groups[2].Value;
            return $"<color=#{colorCode}>{text}</color>";
        });
        
        // Handle text shaking formatting
        // For this, we'll need to track character positions carefully
        string outputText = processedText;
        string shakePattern = @"\*SHAKE\(([^)]*)\)";
        
        // Find all shake matches and process them
        MatchCollection shakeMatches = Regex.Matches(processedText, shakePattern);
        
        // Keep track of character positions as we remove shake tags
        int currentOffset = 0;
        
        // Map to keep track of character position changes after tags are removed
        Dictionary<int, int> positionMap = new Dictionary<int, int>();
        for (int i = 0; i <= processedText.Length; i++)
        {
            positionMap[i] = i;
        }
        
        foreach (Match match in shakeMatches)
        {
            string shakeContent = match.Groups[1].Value;
            
            // Calculate the original positions in the text
            int matchStart = match.Index;
            int matchEnd = match.Index + match.Length;
            
            // Adjust for previous changes
            int adjustedStart = matchStart - currentOffset;
            
            // Calculate visible character indices
            int visibleStartIndex = CountVisibleCharsInRichText(outputText.Substring(0, adjustedStart));
            int visibleLength = CountVisibleCharsInRichText(shakeContent);
            
            // Add the shake range
            shakeRanges.Add(new ShakeRange(visibleStartIndex, visibleLength));
            
            // Remove the shake tag but keep the content
            outputText = outputText.Remove(adjustedStart, match.Length);
            outputText = outputText.Insert(adjustedStart, shakeContent);
            
            // Update the offset for future matches
            currentOffset += (match.Length - shakeContent.Length);
        }
        
        return outputText;
    }
    
    // Count visible characters in rich text (ignoring tags)
    private int CountVisibleCharsInRichText(string text)
    {
        int count = 0;
        bool inTag = false;
        
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '<')
            {
                inTag = true;
            }
            else if (text[i] == '>')
            {
                inTag = false;
            }
            else if (!inTag)
            {
                count++;
            }
        }
        
        return count;
    }
  
    
    // Convert shake ranges to character indices
    private void SetupShakeIndicesFromRanges(List<ShakeRange> shakeRanges)
    {
        shakeIndices.Clear();
        
        foreach (ShakeRange range in shakeRanges)
        {
            for (int i = 0; i < range.length; i++)
            {
                int charIndex = range.startIndex + i;
                if (charIndex < textComponent.textInfo.characterCount)
                {
                    shakeIndices.Add(charIndex);
                }
            }
        }
    }
    
    // Make marked characters wiggle
    private IEnumerator ShakeCharacters()
    {
        while (true)
        {
            // Nothing to shake or not active, just wait
            if (shakeIndices.Count == 0 || !textComponent.gameObject.activeInHierarchy)
            {
                yield return null;
                continue;
            }
            
            textComponent.ForceMeshUpdate();
            TMP_TextInfo textInfo = textComponent.textInfo;
            
            if (textInfo.characterCount == 0)
            {
                yield return null;
                continue;
            }
            
            foreach (int i in shakeIndices)
            {
                // Skip invalid indices
                if (i >= textInfo.characterCount)
                    continue;
                
                // Skip hidden chars
                if (!textInfo.characterInfo[i].isVisible)
                    continue;
                
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                
                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;
                
                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
                
                // Find center point of character
                Vector3 centerPos = (vertices[vertexIndex] + vertices[vertexIndex + 1] + 
                                     vertices[vertexIndex + 2] + vertices[vertexIndex + 3]) / 4f;
                
                // Random offset
                Vector3 offset = new Vector3(
                    Random.Range(-shakeAmount, shakeAmount),
                    Random.Range(-shakeAmount, shakeAmount),
                    0
                );
                
                // Move all 4 verts of the character
                vertices[vertexIndex] = vertices[vertexIndex] - centerPos + (centerPos + offset);
                vertices[vertexIndex + 1] = vertices[vertexIndex + 1] - centerPos + (centerPos + offset);
                vertices[vertexIndex + 2] = vertices[vertexIndex + 2] - centerPos + (centerPos + offset);
                vertices[vertexIndex + 3] = vertices[vertexIndex + 3] - centerPos + (centerPos + offset);
            }
            
            // Apply the changes
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }
            
            yield return new WaitForSeconds(0.05f); // Shake frequency
        }
    }
    
    // Go to next line
    public void AdvanceDialogue()
    {
        if (!dialogueStarted)
            return;
            
        if (isTyping)
        {
            CompleteCurrentLine();
        }
        else
        {
            currentLineIndex++;
            DisplayNextLine();
        }
    }
    
    // Clean up and reset
    public void EndDialogue()
    {
        StopAllCoroutines();
        shakeCoroutine = null;
        
        dialogueStarted = false;
        textComponent.text = string.Empty;
        shakeIndices.Clear();
    }
    
    // Type out text with a delay between characters
    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        textComponent.text = string.Empty;
        shakeIndices.Clear();
        
        // Extract parts for CHANGE command
        bool hasChangeCommand = false;
        string firstPart = line;
        string secondPart = line;
        
        Regex changeRegex = new Regex(@"\*CHANGE\(\(([^)]*)\),\(([^)]*)\)\)");
        Match changeMatch = changeRegex.Match(line);
        
        if (changeMatch.Success)
        {
            hasChangeCommand = true;
            // First part for typing
            firstPart = line.Substring(0, changeMatch.Index) + changeMatch.Groups[1].Value +
                        line.Substring(changeMatch.Index + changeMatch.Length);
            
            // Second part for after typing finishes
            secondPart = line.Substring(0, changeMatch.Index) + changeMatch.Groups[2].Value +
                         line.Substring(changeMatch.Index + changeMatch.Length);
        }
        
        // Process text and get shake ranges
        List<ShakeRange> typingShakeRanges;
        string processedFirstPart = ProcessFormattingWithShake(firstPart, out typingShakeRanges);
        
        // Plain version for counting
        string plainText = Regex.Replace(processedFirstPart, @"<[^>]*>", "");
        
        // Type out each character
        for (int visibleCharCount = 1; visibleCharCount <= plainText.Length; visibleCharCount++)
        {
            string partialText = GetTextUpToVisibleCharCount(processedFirstPart, visibleCharCount);
            textComponent.text = partialText;
            
            textComponent.ForceMeshUpdate();
            
            // Figure out which characters shake in this partial text
            List<ShakeRange> partialShakeRanges = new List<ShakeRange>();
            foreach (ShakeRange range in typingShakeRanges)
            {
                if (range.startIndex < visibleCharCount)
                {
                    int visibleLength = Mathf.Min(range.length, visibleCharCount - range.startIndex);
                    partialShakeRanges.Add(new ShakeRange(range.startIndex, visibleLength));
                }
            }
            
            SetupShakeIndicesFromRanges(partialShakeRanges);
            
            if (shakeIndices.Count > 0 && shakeCoroutine == null)
            {
                shakeCoroutine = StartCoroutine(ShakeCharacters());
            }
            
            yield return new WaitForSeconds(typingSpeed);
        }
        
        // For CHANGE effect, swap to second part
        if (hasChangeCommand)
        {
            List<ShakeRange> secondShakeRanges;
            string processedSecondPart = ProcessFormattingWithShake(secondPart, out secondShakeRanges);
            textComponent.text = processedSecondPart;
            
            textComponent.ForceMeshUpdate();
            SetupShakeIndicesFromRanges(secondShakeRanges);
            
            if (shakeCoroutine != null)
            {
                StopCoroutine(shakeCoroutine);
            }
            
            if (shakeIndices.Count > 0)
            {
                shakeCoroutine = StartCoroutine(ShakeCharacters());
            }
        }
        
        isTyping = false;
    }
    
    // Get text up to a specific visible character count
    private string GetTextUpToVisibleCharCount(string richText, int visibleCharCount)
    {
        StringBuilder result = new StringBuilder();
        int visibleCount = 0;
        bool inTag = false;
        
        for (int i = 0; i < richText.Length; i++)
        {
            char c = richText[i];
            
            if (c == '<')
            {
                inTag = true;
                result.Append(c);
            }
            else if (c == '>')
            {
                inTag = false;
                result.Append(c);
            }
            else if (inTag)
            {
                result.Append(c);
            }
            else
            {
                result.Append(c);
                visibleCount++;
                
                if (visibleCount >= visibleCharCount)
                {
                    // Add any closing tags that might be needed
                    AddClosingTags(richText, i, result);
                    break;
                }
            }
        }
        
        return result.ToString();
    }
    
    // Make sure all open tags are closed in partial text
    private void AddClosingTags(string fullText, int currentPos, StringBuilder partialText)
    {
        Stack<string> openTags = new Stack<string>();
        
        // Find all open tags up to the current position
        int pos = 0;
        while (pos <= currentPos)
        {
            int tagStart = fullText.IndexOf('<', pos);
            if (tagStart == -1 || tagStart > currentPos)
                break;
            
            int tagEnd = fullText.IndexOf('>', tagStart);
            if (tagEnd == -1)
                break;
            
            string tag = fullText.Substring(tagStart + 1, tagEnd - tagStart - 1);
            
            if (tag.StartsWith("/"))
            {
                // Close tag - pop matching open tag
                if (openTags.Count > 0)
                {
                    openTags.Pop();
                }
            }
            else
            {
                // Open tag - add to stack
                openTags.Push(tag);
            }
            
            pos = tagEnd + 1;
        }
        
        // Add closing tags in reverse order
        while (openTags.Count > 0)
        {
            string tag = openTags.Pop();
            // Just the tag name, no attributes
            string tagName = tag.Split(' ')[0];
            partialText.Append($"</{tagName}>");
        }
    }
}