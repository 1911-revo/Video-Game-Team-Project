using UnityEngine;

public class bulletMovement : MonoBehaviour
{

    private float speed = 4.0f;


    //public bool facing;
    private SpriteRenderer bulletSpriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // public void setFacing(bool f){
    //     facing = f;
    // }

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
