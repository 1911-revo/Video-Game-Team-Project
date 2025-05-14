using System;
using UnityEngine;

public class bulletMovement : MonoBehaviour
{

    //private float speed = 4.0f;



    //public bool facing;
    private SpriteRenderer bulletSpriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // public void setFacing(bool f){
    //     facing = f;
    // }

    // This function is called when the bullet collides with another collider
    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject); // Destroy the bullet
        Debug.Log("Collision with: " + collision.gameObject.name);
        // Check if the bullet collided with the crate
        if (collision.gameObject.CompareTag("Crate")) // Ensure your crate has a "Crate" tag
        {
            Destroy(collision.gameObject); // Destroy the crate
            
        }
    }

    void Start()
    {
        bulletSpriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {

        //Note: The projectile moves "left" when going right because the bullet asset is flipped
        // if (facing == true){
        //     transform.Translate(Vector2.left * Time.deltaTime * speed);
        // } else {
        //     transform.Translate(Vector2.right * Time.deltaTime * speed);
        // }
        
        
    }
    private void FixedUpdate()
    {
        SetFacingDirection();
    }

    private void SetFacingDirection()
    {
        
        // if (facing == false)
        // {
        //     bulletSpriteRenderer.flipX = true;
        // } 
        // else
        // {
        //     bulletSpriteRenderer.flipX = false;
        // }
    }
}
