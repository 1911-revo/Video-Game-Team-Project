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
        movement = playerControls.Player.Move.ReadValue<Vector2>();

        if (movement.magnitude > 0)
        {
            movement.Normalize();
        }

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
    /// <summary>
    /// action to open (display) the inventory UI when the corresponding input is detected.
    /// </summary>



    private void OpenInventory(InputAction.CallbackContext context)
    {
        if (inventoryAnimator == null) return;

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


}
