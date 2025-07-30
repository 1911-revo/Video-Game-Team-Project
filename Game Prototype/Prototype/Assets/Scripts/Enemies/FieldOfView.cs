using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

/// <summary>
/// Class to display and manage entities field of view
/// </summary>
public class FieldOfView : MonoBehaviour
{
    [Header("Configuration")]
    [Range(0,360)]
    [SerializeField] public float fov = 90f;
    [Range(0,20)]
    [SerializeField] public float viewDistance;
    [Range(2,100)]
    [SerializeField] private int rayCount = 2;

    [Header("Interactable Layers")]
    [SerializeField] private LayerMask environmentLayerMask;
    [SerializeField] private LayerMask targetLayerMask;

    private Mesh mesh;
    private LayerMask combinedLayerMask;
    private Vector3 origin;
    private float startingAngle;
    private float targetAngle;
    private float rotationSpeed;

    [HideInInspector] public float percentRaysOnPlayer;

    private void Start()
    {
        // Initialise 
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        combinedLayerMask = environmentLayerMask | targetLayerMask;
    }

    // LateUpdate is called after all Update calls. 
    // Field of view depends on objects that can move in update so this ensures correct results
    private void LateUpdate()
    {
        DrawViewCone();   
    }

    /// <summary>
    /// Create and display the view cone and check how much of the player can be seen
    /// </summary>
    private void DrawViewCone()
    {
        // Reset variables
        float raysOnPlayer = 0;
        float angle = startingAngle;
        float angleIncrease = fov / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 2];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        // Create view cone mesh
        int vertexIdx = 1;
        int triangleIdx = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;
            // Cast ray to check for objects within viewDistance
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, VectorFromAngle(angle), viewDistance, combinedLayerMask);
            if (raycastHit2D.collider == null) // No hit
            {
                vertex = origin + VectorFromAngle(angle) * viewDistance; // Can see to max distance
            }
            else if (raycastHit2D.collider.gameObject.CompareTag("Player")) // Hit target
            {
                vertex = origin + VectorFromAngle(angle) * viewDistance; // Don't change view cone for target
                raysOnPlayer++;
            }
            else // Hit environment obstacle
            {
                vertex = raycastHit2D.point; // Can't see past obstacle
            }
            vertices[vertexIdx] = vertex;

            // Create triangle for mesh
            if (i > 0)
            {
                triangles[triangleIdx + 0] = 0;
                triangles[triangleIdx + 1] = vertexIdx - 1;
                triangles[triangleIdx + 2] = vertexIdx;

                triangleIdx += 3;
            }
            vertexIdx++;

            // Increment angle for next ray
            angle += angleIncrease;
        }
        // Compute how much of player is seen
        percentRaysOnPlayer = raysOnPlayer / rayCount;

        // Display view cone
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }

    /// <summary>
    /// Helper function to convert angle to vector
    /// </summary>
    /// <param name="angle"> Angle following Unity convention </param>
    /// <returns> Direction vector corresponding to angle </returns>
    public static Vector3 VectorFromAngle(float angle)
    {
        float angleRad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    /// <summary>
    /// Helper function to convert vector to angle
    /// </summary>
    /// <param name="dir"> Direction vector </param>
    /// <returns> Angle following Unity convention corresponding to dir </returns>
    public static float AngleFromVector(Vector3 dir)
    {
        dir = dir.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;
        return angle;
    }

    /// <summary>
    /// Set the origin of the view cone
    /// </summary>
    /// <param name="origin"> Origin of the view cone </param>
    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }

    /// <summary>
    /// Set the desired target direction of the enemy and the speed to rotate the 
    /// view cone at to get to this direction
    /// </summary>
    /// <param name="direction"> Target direction of the enemies view cone </param>
    /// <param name="speed"> Speed at which the view cone will be rotated towards the target direction </param>
    public void SetViewDirection(Vector3 direction, float speed)
    {
        rotationSpeed = speed;
        targetAngle = AngleFromVector(direction) - fov / 2f;
    }

    /// <summary>
    /// Move the direction of the view cone towards the target direction at rotationSpeed
    /// </summary>
    public void RotateViewCone()
    {
        startingAngle = Mathf.MoveTowardsAngle(startingAngle, targetAngle, rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Get the starting angle of the current view cone
    /// </summary>
    /// <returns> Starting angle of view cone </returns>
    public float GetCurrentAngle()
    {
        return startingAngle;
    }
}