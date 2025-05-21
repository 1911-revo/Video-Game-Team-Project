using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [SerializeField] string tagFilter = "Player";

    [SerializeField] UnityEvent onTriggerEnter;
    [SerializeField] UnityEvent onTriggerExit;

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (!string.IsNullOrEmpty(tagFilter) && !collision.CompareTag(tagFilter)) return;
        onTriggerEnter.Invoke();
    }

    private void OnTriggerExit2D(UnityEngine.Collider2D collision)
    {
        if (!string.IsNullOrEmpty(tagFilter) && !collision.CompareTag(tagFilter)) return;
        onTriggerExit.Invoke();
    }
}

