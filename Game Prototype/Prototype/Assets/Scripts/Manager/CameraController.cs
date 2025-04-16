using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform playerTransform;
    [Tooltip("How fast the camera moves to follow the player")]
    [SerializeField] private float followSpeed = 5f;

    [Header("Cursor Influence")]
    [Tooltip("How much the cursor position affects camera offset")]
    [SerializeField] private float cursorInfluence = 0.2f;
    [Tooltip("Maximum distance the camera can be offset from the player")]
    [SerializeField] private float maxCursorOffset = 3f;

    [Header("Smoothing")]
    [Tooltip("Whether to use smoothing for cursor influence")]
    [SerializeField] private bool useSmoothCursorOffset = true;
    [Tooltip("How quickly the camera adjusts to cursor position changes")]
    [SerializeField] private float cursorSmoothSpeed = 3f;


    // Variables for calculating where the camera should be looking
    private Camera mainCamera;
    private Vector2 currentCursorOffset;
    private Vector2 targetCursorOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Pulls the main camera despite what the camera's name is. (So long as it has the MainCamera tag)
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("No camera with MainCamera tag found!");
            enabled = false;
            return;
        }
    

     if (playerTransform == null)
        {
            Debug.LogError("Player Transform not assigned to CameraController!");
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            
            if (playerTransform == null)
            {
                Debug.LogError("Could not find player with tag 'Player'. Please assign player transform manually.");
                enabled = false;
                return;
            }
        }
    }

    private void LateUpdate()
    {
        if (playerTransform == null) return;

        // Calculate cursor influence
        CalculateCursorOffset();

        // Calculate target position (player position + cursor offset)
        Vector3 targetPosition = new Vector3(
            playerTransform.position.x + currentCursorOffset.x,
            playerTransform.position.y + currentCursorOffset.y,
            transform.position.z  // Maintain camera z-position
        );

        // Smoothly move camera to target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    // Calculates where the camera should be looking between the player and the cursor.
    private void CalculateCursorOffset()
    {
        // Convert mouse position to world point (optimized for 2D)
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPos = playerTransform.position;

        // Get direction from player to cursor
        Vector2 cursorDir = mouseWorldPos - playerPos;

        // Calculate cursor offset based on direction and magnitude
        float distance = cursorDir.magnitude;
        Vector2 normalizedDir = distance > 0 ? cursorDir / distance : Vector2.zero;

        // Apply curve to the influence (can be modified for different feel)
        float influence = Mathf.Min(distance * cursorInfluence, maxCursorOffset);

        // Set target offset
        targetCursorOffset = normalizedDir * influence;

        // Apply smoothing if enabled
        if (useSmoothCursorOffset)
        {
            currentCursorOffset = Vector2.Lerp(currentCursorOffset, targetCursorOffset, cursorSmoothSpeed * Time.deltaTime);
        }
        else
        {
            currentCursorOffset = targetCursorOffset;
        }
    }
}
