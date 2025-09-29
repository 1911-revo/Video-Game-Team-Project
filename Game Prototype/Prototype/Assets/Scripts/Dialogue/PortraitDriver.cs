using UnityEngine;

public class PortraitDriver : MonoBehaviour
{
    [SerializeField] private Animator portraitAnimator;

    public void SetEmotion(int emotion)
    {
        if (portraitAnimator != null)
            portraitAnimator.SetInteger("Emotion", emotion);
    }

}
