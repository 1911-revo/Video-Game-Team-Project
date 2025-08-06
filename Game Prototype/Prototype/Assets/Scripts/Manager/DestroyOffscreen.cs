using UnityEngine;

public class DestroyOffscreen : MonoBehaviour
{
    private float rightBound = 15;
    private float leftBound = -15;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > rightBound){
            Destroy(gameObject);
        } else if (transform.position.x < leftBound){
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
