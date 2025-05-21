using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;


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
    [SerializeField] private GameObject inventoryUI; // Reference to the inventory UI


    // Input system controls
    private PlayerControls playerControls;
    private Vector2 movement;
    private InputAction openInventoryAction; // Action to open the inventory
    private Animator inventoryAnimator; // Animator to control inventory animation
    private bool inventoryIsOpen = false;

    // Added variable to control movement
    private bool canMove = true;


    /// <summary>
    /// Initialize input controls.
    /// </summary>
    private void Awake()
    {
        playerControls = new PlayerControls();
        openInventoryAction = playerControls.Player.OpenInventory;

        if (inventoryUI != null)
        {
            inventoryAnimator = inventoryUI.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("inventoryUI is null! Please assign it in Inspector.");
        }

        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }


    /// <summary>
    /// Enable input controls when the object is enabled.
    /// </summary>
    private void OnEnable()
    {
        playerControls.Enable();
        openInventoryAction.performed += OpenInventory;
    }

    private void OnDisable()
    {
        if (openInventoryAction != null)
        {
            openInventoryAction.performed -= OpenInventory;
        }

        if (playerControls != null)
        {
            playerControls.Disable();
        }
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
        // Only read movement input if player can move
        if (canMove)
        {
            movement = playerControls.Player.Move.ReadValue<Vector2>();

            if (movement.magnitude > 0)
            {
                movement.Normalize();
            }
        }
        else
        {
            // Zero out movement when player can't move
            movement = Vector2.zero;
        }

        playerAnimator.SetFloat("moveX", movement.x);
        playerAnimator.SetFloat("moveY", movement.y);
    }

    /// <summary>
    /// Moves the player character based on input.
    /// </summary>
    private void Move()
    {
        // Only apply movement if player can move
        if (canMove)
        {
            rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
        }
    }

    /// <summary>
    /// Flips the player's sprite to face the direction of the cursor.
    /// </summary>
    private void SetFacingDirection()
    {
        // Even when movement is disabled, we still want the player to face the cursor
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

    /// <summary>
    /// Action to open (display) the inventory UI when the corresponding input is detected.
    /// </summary>
    private void OpenInventory(InputAction.CallbackContext context)
    {
        // Only allow opening inventory if player can move (not in dialogue)
        if (inventoryAnimator == null || !canMove) return;

        inventoryIsOpen = !inventoryIsOpen;

        if (inventoryIsOpen)
        {
            inventoryAnimator.SetTrigger("TriggerOpen");
            Debug.Log("Triggered Open");
        }
        else
        {
            inventoryAnimator.SetTrigger("TriggerClose");
            Debug.Log("Triggered Close");
        }
    }

    /// <summary>
    /// Enables or disables player movement.
    /// </summary>
    /// <param name="enabled">Whether movement should be enabled or disabled.</param>
    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;

        // If disabling movement, also reset current movement to prevent sliding
        if (!enabled)
        {
            movement = Vector2.zero;
            playerAnimator.SetFloat("moveX", 0);
            playerAnimator.SetFloat("moveY", 0);

            // Optional: Stop the rigidbody velocity as well
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}