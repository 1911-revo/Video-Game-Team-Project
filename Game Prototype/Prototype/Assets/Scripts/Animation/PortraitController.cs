using UnityEngine;
using TMPro;

public class PortraitController : MonoBehaviour
{
    [SerializeField] private Animator portraitAnimator;
    [SerializeField] private TextMeshProUGUI nameText;

    public void SetName(string characterName)
    {
        if (nameText != null)
            nameText.text = characterName;
    }
    // Play the specified animation
    public void PlayAnimation(string animationName)
    {
        if (portraitAnimator != null)
            portraitAnimator.Play(animationName, 0, 0f);// Play from the start
    }
}
