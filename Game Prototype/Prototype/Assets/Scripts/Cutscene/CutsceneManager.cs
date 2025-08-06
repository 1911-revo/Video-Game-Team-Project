using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public abstract class CutsceneAction
{
    public float delay = 0f;
    public bool waitForCompletion = true;

    public abstract IEnumerator Execute(CutsceneManager manager);

    public virtual string GetActionName()
    {
        return GetType().Name.Replace("Action", "");
    }
}

[Serializable]
public class DialogueAction : CutsceneAction
{
    [Header("Dialogue Source")]
    public NPCDialogue npcToTalk;
    public DialogueTree customDialogueTree;
    public bool useCustomTree = false;

    [Header("Dialogue Node Settings")]
    public string startNodeID = "";
    public bool markNPCAsSpoken = true;

    public override IEnumerator Execute(CutsceneManager manager)
    {
        yield return new WaitForSeconds(delay);

        Debug.Log("=== DialogueAction: Starting dialogue ===");

        if (useCustomTree && customDialogueTree != null)
        {
            StartCustomDialogue(manager);
        }
        else if (npcToTalk != null)
        {
            if (markNPCAsSpoken)
            {
                manager.dialogueManager.StartDialogue(npcToTalk);
            }
            else
            {
                StartNPCDialogueWithoutMarking(manager);
            }
        }

        if (waitForCompletion)
        {
            Debug.Log("DialogueAction: Waiting for dialogue tree completion...");
            
            yield return null;
            
            while (manager.dialogueManager.IsDialogueActive())
            {
                yield return null;
            }
            
            while (manager.dialogueController != null && manager.dialogueController.dialogueStarted)
            {
                yield return null;
            }

            Debug.Log("DialogueAction: Dialogue tree completed!");

            if (manager.dialogueManager != null)
            {
                manager.lastDialogueChoice = manager.dialogueManager.GetLastSelectedNodeID();
                Debug.Log($"DialogueAction: Stored last choice: {manager.lastDialogueChoice}");
            }
        }

        Debug.Log("=== DialogueAction: Execute completed ===");
    }

    private void StartCustomDialogue(CutsceneManager manager)
    {
        if (npcToTalk != null)
        {
            DialogueTree originalTree = npcToTalk.dialogueTree;
            bool originalUseTree = npcToTalk.useDialogueTree;

            npcToTalk.dialogueTree = customDialogueTree;
            npcToTalk.useDialogueTree = true;

            if (!string.IsNullOrEmpty(startNodeID))
            {
                manager.dialogueManager.StartDialogueFromNode(npcToTalk, startNodeID);
            }
            else
            {
                manager.dialogueManager.StartDialogue(npcToTalk);
            }

            manager.StartCoroutine(RestoreOriginalTree(npcToTalk, originalTree, originalUseTree));
        }
        else
        {
            Debug.LogError("Custom dialogue tree requires an NPC to be assigned!");
        }
    }

    private IEnumerator RestoreOriginalTree(NPCDialogue npc, DialogueTree originalTree, bool originalUseTree)
    {
        yield return null;
        npc.dialogueTree = originalTree;
        npc.useDialogueTree = originalUseTree;
    }

    private void StartNPCDialogueWithoutMarking(CutsceneManager manager)
    {
        bool originalSpokenState = npcToTalk.HasSpokenBefore();
        manager.dialogueManager.StartDialogue(npcToTalk);

        if (!originalSpokenState)
        {
            manager.StartCoroutine(ResetSpokenStateNextFrame(npcToTalk));
        }
    }

    private IEnumerator ResetSpokenStateNextFrame(NPCDialogue npc)
    {
        yield return null;
        npc.ResetSpokenState();
    }
}

[Serializable]
public class MoveAction : CutsceneAction
{
    public Transform characterToMove;
    public Transform targetPosition;
    public Vector3 targetOffset;
    public bool useOffset = false;
    public float moveSpeed = 3f;
    public AnimationCurve movementCurve = AnimationCurve.Linear(0, 0, 1, 1);

    public override IEnumerator Execute(CutsceneManager manager)
    {
        yield return new WaitForSeconds(delay);

        if (characterToMove != null)
        {
            Vector3 startPos = characterToMove.position;
            Vector3 endPos = useOffset ? startPos + targetOffset : targetPosition.position;

            Animator animator = characterToMove.GetComponent<Animator>();
            bool hasAnimator = animator != null;

            float elapsedTime = 0f;
            float distance = Vector3.Distance(startPos, endPos);
            float duration = distance / moveSpeed;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                float curveValue = movementCurve.Evaluate(t);

                characterToMove.position = Vector3.Lerp(startPos, endPos, curveValue);

                if (hasAnimator && elapsedTime < duration - 0.1f)
                {
                    Vector3 direction = (endPos - startPos).normalized;
                    animator.SetFloat("moveX", direction.x);
                    animator.SetFloat("moveY", direction.y);
                }

                if (!waitForCompletion)
                {
                    yield break;
                }

                yield return null;
            }

            characterToMove.position = endPos;

            if (hasAnimator)
            {
                animator.SetFloat("moveX", 0);
                animator.SetFloat("moveY", 0);
            }
        }
    }
}

[Serializable]
public class CameraFocusAction : CutsceneAction
{
    public Transform focusTarget;
    public float focusDuration = 1f;
    public float cameraSize = 5f;
    public bool changeCameraSize = false;
    public AnimationCurve focusCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public Vector3 offset = Vector3.zero;

    public override IEnumerator Execute(CutsceneManager manager)
    {
        yield return new WaitForSeconds(delay);

        if (focusTarget != null && manager.mainCamera != null)
        {
            Transform camTransform = manager.mainCamera.transform;
            Vector3 startPos = camTransform.position;
            Vector3 targetPos = focusTarget.position + offset;
            targetPos.z = startPos.z;

            float startSize = manager.mainCamera.orthographicSize;
            float targetSize = changeCameraSize ? cameraSize : startSize;

            float elapsedTime = 0f;

            if (manager.cameraController != null)
            {
                manager.cameraController.enabled = false;
            }

            while (elapsedTime < focusDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / focusDuration;
                float curveValue = focusCurve.Evaluate(t);

                camTransform.position = Vector3.Lerp(startPos, targetPos, curveValue);

                if (changeCameraSize)
                {
                    manager.mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, curveValue);
                }

                if (!waitForCompletion)
                {
                    yield break;
                }

                yield return null;
            }

            camTransform.position = targetPos;
            if (changeCameraSize)
            {
                manager.mainCamera.orthographicSize = targetSize;
            }
        }
    }
}

[Serializable]
public class AnimationAction : CutsceneAction
{
    public Animator targetAnimator;
    public string triggerName;
    public string boolName;
    public bool boolValue;
    public bool useTrigger = true;
    public float animationDuration = 1f;

    public override IEnumerator Execute(CutsceneManager manager)
    {
        yield return new WaitForSeconds(delay);

        if (targetAnimator != null)
        {
            if (useTrigger && !string.IsNullOrEmpty(triggerName))
            {
                targetAnimator.SetTrigger(triggerName);
            }
            else if (!string.IsNullOrEmpty(boolName))
            {
                targetAnimator.SetBool(boolName, boolValue);
            }

            if (waitForCompletion)
            {
                yield return new WaitForSeconds(animationDuration);
            }
        }
    }
}

[Serializable]
public class WaitAction : CutsceneAction
{
    public float waitDuration = 1f;

    public override IEnumerator Execute(CutsceneManager manager)
    {
        yield return new WaitForSeconds(delay + waitDuration);
    }
}

[Serializable]
public class ConditionalAction : CutsceneAction
{
    public enum ConditionType
    {
        LastDialogueChoice,
        CustomCondition
    }

    [Header("Condition Settings")]
    public ConditionType conditionType = ConditionType.LastDialogueChoice;
    public string expectedChoiceNodeID = "";

    [Header("Actions")]
    [SerializeReference] public List<CutsceneAction> actionsIfTrue = new List<CutsceneAction>();
    [SerializeReference] public List<CutsceneAction> actionsIfFalse = new List<CutsceneAction>();

    [Header("Custom Condition")]
    public UnityEvent<ConditionalAction> customConditionCheck;
    private bool conditionResult = false;

    public void SetConditionResult(bool result)
    {
        conditionResult = result;
    }

    public override IEnumerator Execute(CutsceneManager manager)
    {
        yield return new WaitForSeconds(delay);

        bool condition = false;

        switch (conditionType)
        {
            case ConditionType.LastDialogueChoice:
                condition = manager.lastDialogueChoice == expectedChoiceNodeID;
                break;

            case ConditionType.CustomCondition:
                customConditionCheck?.Invoke(this);
                condition = conditionResult;
                break;
        }

        List<CutsceneAction> actionsToExecute = condition ? actionsIfTrue : actionsIfFalse;

        foreach (var action in actionsToExecute)
        {
            if (action != null)
            {
                yield return manager.StartCoroutine(action.Execute(manager));
            }
        }
    }
}

[Serializable]
public class Cutscene
{
    public string cutsceneName = "New Cutscene";

    [SerializeReference]
    public List<CutsceneAction> actions = new List<CutsceneAction>();

    public bool disablePlayerMovement = true;
    public bool returnCameraToPlayer = true;
    public UnityEvent onCutsceneStart;
    public UnityEvent onCutsceneEnd;
}

public class CutsceneManager : MonoBehaviour
{
    [Header("References")]
    public DialogueManager dialogueManager;
    public Dialogue dialogueController;
    public CameraController cameraController;
    public PlayerController playerController;
    public Camera mainCamera;

    [Header("Cutscenes")]
    [SerializeField] public List<Cutscene> cutscenes = new List<Cutscene>();

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    [SerializeField] private int testCutsceneIndex = 0;

    private bool isPlayingCutscene = false;
    private Coroutine currentCutsceneCoroutine;

    public string lastDialogueChoice { get; set; }
    private Dictionary<string, string> dialogueChoiceHistory = new Dictionary<string, string>();

    void Start()
    {
        if (dialogueManager == null) dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueController == null) dialogueController = FindObjectOfType<Dialogue>();
        if (cameraController == null) cameraController = FindObjectOfType<CameraController>();
        if (playerController == null) playerController = FindObjectOfType<PlayerController>();
        if (mainCamera == null) mainCamera = Camera.main;

        foreach (var cutscene in cutscenes)
        {
            if (cutscene.actions == null)
            {
                cutscene.actions = new List<CutsceneAction>();
            }
        }

        if (dialogueManager == null) Debug.LogWarning("DialogueManager not found!");
        if (dialogueController == null) Debug.LogWarning("Dialogue controller not found!");
    }

    void Update()
    {
        if (debugMode && Input.GetKeyDown(KeyCode.T))
        {
            PlayCutscene(testCutsceneIndex);
        }
    }

    public void PlayCutscene(int index)
    {
        if (index >= 0 && index < cutscenes.Count)
        {
            PlayCutscene(cutscenes[index]);
        }
        else
        {
            Debug.LogError($"Cutscene index {index} out of range!");
        }
    }

    public void PlayCutscene(string name)
    {
        Cutscene cutscene = cutscenes.Find(c => c.cutsceneName == name);
        if (cutscene != null)
        {
            PlayCutscene(cutscene);
        }
        else
        {
            Debug.LogError($"Cutscene '{name}' not found!");
        }
    }

    public void PlayCutscene0() => PlayCutscene(0);
    public void PlayCutscene1() => PlayCutscene(1);
    public void PlayCutscene2() => PlayCutscene(2);
    public void PlayCutscene3() => PlayCutscene(3);
    public void PlayCutscene4() => PlayCutscene(4);

    public void PlayCutscene(Cutscene cutscene)
    {
        if (isPlayingCutscene)
        {
            Debug.LogWarning("Already playing a cutscene!");
            return;
        }

        currentCutsceneCoroutine = StartCoroutine(PlayCutsceneCoroutine(cutscene));
    }

    public void StopCutscene()
    {
        if (currentCutsceneCoroutine != null)
        {
            StopCoroutine(currentCutsceneCoroutine);
            currentCutsceneCoroutine = null;
        }

        isPlayingCutscene = false;

        if (playerController != null)
        {
            playerController.SetMovementEnabled(true);
        }

        if (cameraController != null)
        {
            cameraController.enabled = true;
        }
    }

    private IEnumerator PlayCutsceneCoroutine(Cutscene cutscene)
    {
        isPlayingCutscene = true;

        cutscene.onCutsceneStart?.Invoke();

        if (cutscene.disablePlayerMovement && playerController != null)
        {
            playerController.SetMovementEnabled(false);
        }

        foreach (var action in cutscene.actions)
        {
            if (action != null)
            {
                if (debugMode)
                {
                    Debug.Log($"Executing cutscene action: {action.GetType().Name}");
                }

                yield return StartCoroutine(action.Execute(this));
            }
        }

        if (cutscene.returnCameraToPlayer && cameraController != null && playerController != null)
        {
            yield return StartCoroutine(ReturnCameraToPlayer());
        }

        if (cutscene.disablePlayerMovement && playerController != null)
        {
            playerController.SetMovementEnabled(true);
        }

        if (cameraController != null)
        {
            cameraController.enabled = true;
        }

        cutscene.onCutsceneEnd?.Invoke();

        isPlayingCutscene = false;
        currentCutsceneCoroutine = null;
    }

    private IEnumerator ReturnCameraToPlayer()
    {
        if (mainCamera != null && playerController != null)
        {
            Transform camTransform = mainCamera.transform;
            Vector3 startPos = camTransform.position;
            Vector3 targetPos = playerController.transform.position;
            targetPos.z = startPos.z;

            float duration = 1f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                camTransform.position = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }

            camTransform.position = targetPos;
        }
    }

    public bool IsPlayingCutscene()
    {
        return isPlayingCutscene;
    }

    public int GetCutsceneCount()
    {
        return cutscenes.Count;
    }

    public void StopCurrentAndPlayNew(int index, string name)
    {
        StopCutscene();

        if (index >= 0)
        {
            PlayCutscene(index);
        }
        else if (!string.IsNullOrEmpty(name))
        {
            PlayCutscene(name);
        }
    }

    public void StoreDialogueChoice(string choiceKey, string choiceValue)
    {
        dialogueChoiceHistory[choiceKey] = choiceValue;
    }

    public string GetStoredChoice(string choiceKey)
    {
        return dialogueChoiceHistory.ContainsKey(choiceKey) ? dialogueChoiceHistory[choiceKey] : "";
    }

    [ContextMenu("Add Dialogue Action")]
    void AddDialogueAction()
    {
        if (cutscenes.Count > 0)
        {
            cutscenes[0].actions.Add(new DialogueAction());
        }
    }

    [ContextMenu("Add Move Action")]
    void AddMoveAction()
    {
        if (cutscenes.Count > 0)
        {
            cutscenes[0].actions.Add(new MoveAction());
        }
    }

    [ContextMenu("Add Camera Focus Action")]
    void AddCameraFocusAction()
    {
        if (cutscenes.Count > 0)
        {
            cutscenes[0].actions.Add(new CameraFocusAction());
        }
    }

    [ContextMenu("Add Animation Action")]
    void AddAnimationAction()
    {
        if (cutscenes.Count > 0)
        {
            cutscenes[0].actions.Add(new AnimationAction());
        }
    }

    [ContextMenu("Add Wait Action")]
    void AddWaitAction()
    {
        if (cutscenes.Count > 0)
        {
            cutscenes[0].actions.Add(new WaitAction());
        }
    }

    [ContextMenu("Add Conditional Action")]
    void AddConditionalAction()
    {
        if (cutscenes.Count > 0)
        {
            cutscenes[0].actions.Add(new ConditionalAction());
        }
    }

    // Helper methods to add actions to specific cutscenes
    public void AddDialogueActionToCutscene(int cutsceneIndex)
    {
        if (cutsceneIndex >= 0 && cutsceneIndex < cutscenes.Count)
        {
            cutscenes[cutsceneIndex].actions.Add(new DialogueAction());
        }
    }

    public void AddMoveActionToCutscene(int cutsceneIndex)
    {
        if (cutsceneIndex >= 0 && cutsceneIndex < cutscenes.Count)
        {
            cutscenes[cutsceneIndex].actions.Add(new MoveAction());
        }
    }

    public void AddCameraFocusActionToCutscene(int cutsceneIndex)
    {
        if (cutsceneIndex >= 0 && cutsceneIndex < cutscenes.Count)
        {
            cutscenes[cutsceneIndex].actions.Add(new CameraFocusAction());
        }
    }

    public void AddAnimationActionToCutscene(int cutsceneIndex)
    {
        if (cutsceneIndex >= 0 && cutsceneIndex < cutscenes.Count)
        {
            cutscenes[cutsceneIndex].actions.Add(new AnimationAction());
        }
    }

    public void AddWaitActionToCutscene(int cutsceneIndex)
    {
        if (cutsceneIndex >= 0 && cutsceneIndex < cutscenes.Count)
        {
            cutscenes[cutsceneIndex].actions.Add(new WaitAction());
        }
    }

    public void AddConditionalActionToCutscene(int cutsceneIndex)
    {
        if (cutsceneIndex >= 0 && cutsceneIndex < cutscenes.Count)
        {
            cutscenes[cutsceneIndex].actions.Add(new ConditionalAction());
        }
    }
}