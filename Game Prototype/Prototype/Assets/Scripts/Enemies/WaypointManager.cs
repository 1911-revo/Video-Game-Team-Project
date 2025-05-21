using UnityEngine;

/// <summary>
/// Class to manage the waypoints of a patrolling enemy
/// </summary>
public class WaypointManager : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject[] waypoints;
    private int currentPoint; // point to position in array

    [Header("Gizmo parameters")] // For visualisation in scene
    [SerializeField] private Color pointColour = Color.red;
    [SerializeField] private float pointSize = 0.1f;

    /// <summary>
    /// Get the waypoint currently pointed to
    /// </summary>
    /// <returns> Transform of the current waypoint </returns>
    public Transform CurrentWaypoint()
    {
        return waypoints[currentPoint].transform;
    }

    /// <summary>
    /// Increase the position pointer by 1
    /// Loop back to start if at the end of array
    /// </summary>
    public void GetNextWaypoint()
    {
        currentPoint++;
        if (currentPoint >= waypoints.Length)
        {
            currentPoint = 0;
        }
    }

    /// <summary>
    /// Helper function to display waypoint locations in scene for debugging and level building
    /// </summary>
    private void OnDrawGizmos()
    {
        if (waypoints.Length == 0) 
            return;
        
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.color = pointColour;
                Gizmos.DrawSphere(waypoints[i].transform.position, pointSize);
            }
        }
    }
}
