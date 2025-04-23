using UnityEngine;

/// <summary>
/// Controls the player character's movement and animation.
/// </summary>
public class PlayerController : MonoBehaviour
{
    // References and settings for movement and animation
    [SerializeField] private float moveSpeed; 
    [SerializeField] private Rigidbody2D rb; 
    [SerializeField] private Animator playerAnimator; 
    [SerializeField] private SpriteRenderer playerSpriteRenderer;

    // Input system controls
    private PlayerControls playerControls; 
    private Vector2 movement; 

    /// <summary>
    /// Initialize input controls.
    /// </summary>
    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    /// <summary>
    /// Enable input controls when the object is enabled.
    /// </summary>
    private void OnEnable()
    {
        playerControls.Enable();
    }

    /// <summary>
    /// Called every frame to get input from the player.
    /// </summary>
    private void Update()
    {
        PlayerInput();
    }

    /// <summary>
    /// Called at a fixed time interval to handle physics-related updates.
    /// </summary>
    private void FixedUpdate()
    {
        SetFacingDirection();
        Move();
    }

    /// <summary>
    /// Reads the player's input and updates animation parameters accordingly.
    /// </summary>
    private void PlayerInput()
    {
        movement = playerControls.Player.Move.ReadValue<Vector2>();

        playerAnimator.SetFloat("moveX", movement.x);
        playerAnimator.SetFloat("moveY", movement.y);
    }

    /// <summary>
    /// Moves the player character based on input.
    /// </summary>
    private void Move()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    /// <summary>
    /// Flips the player's sprite to face the direction of the cursor.
    /// </summary>
    private void SetFacingDirection()
    {
        Vector3 cursorPos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (cursorPos.x < playerScreenPoint.x)
        {
            playerSpriteRenderer.flipX = true;
        } 
        else
        {
            playerSpriteRenderer.flipX = false;
        }
    }
}
