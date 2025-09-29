using UnityEngine;

public class DestroyOffscreen : MonoBehaviour
{
    private float rightBound = 15f;
    private float leftBound = -15f;

    void Update()
    {
        if (transform.position.x > rightBound || transform.position.x < leftBound)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
    }
}