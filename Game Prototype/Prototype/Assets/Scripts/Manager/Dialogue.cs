using System.Collections;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private float typingSpeed = 0.05f;


    private string[] dialogueLines;
    private int currentLineIndex;
    private bool isTyping;
    private bool dialogueStarted;
    private Coroutine typingCoroutine;


    string[] testDialogue = new string[]
{
    "Hello, this is test dialogue.",
    "Some of this text should appear *#FF0000(red).",
    "This text should *SHAKE(shake).",
    "This text should be written, and then *CHANGE((change.),(Stay exactly the same)).",
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
        // Check for Enter/Return key press
        if (dialogueStarted && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            if (isTyping)
            {
                // If text is still typing out, complete it instantly
                CompleteCurrentLine();
            }
            else
            {
                // Move to the next line
                currentLineIndex++;
                DisplayNextLine();
            }
        }
    }

    // Public method to be called from another script to start the dialogue
    public void StartDialogue(string[] lines)
    {
        dialogueLines = lines;
        currentLineIndex = 0;
        dialogueStarted = true;

        DisplayNextLine();
    }

    // Display the next line in the sequence
    public void DisplayNextLine()
    {
        if (!dialogueStarted)
            return;

        // If we've reached the end of the dialogue
        if (currentLineIndex > dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        // If currently typing, complete the line instantly
        if (isTyping)
        {
            CompleteCurrentLine();
            return;
        }

        // Start typing the next line
        typingCoroutine = StartCoroutine(TypeLine(dialogueLines[currentLineIndex]));
    }

    // Instantly show the full current line
    public void CompleteCurrentLine()
    {
        if (isTyping && typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            isTyping = false;
            textComponent.text = dialogueLines[currentLineIndex];
        }
    }

    // Advance to the next dialogue line
    public void AdvanceDialogue()
    {
        if (!dialogueStarted)
            return;

        if (isTyping)
        {
            // If text is still typing out, complete it instantly
            CompleteCurrentLine();
        }
        else
        {
            // Move to the next line
            currentLineIndex++;
            DisplayNextLine();
        }
    }

    // End the dialogue sequence
    public void EndDialogue()
    {
        dialogueStarted = false;
        textComponent.text = string.Empty;
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        textComponent.text = string.Empty;

        foreach (char c in line.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

}
