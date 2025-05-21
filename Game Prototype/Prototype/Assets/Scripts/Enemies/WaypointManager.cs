using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject[] waypoints;
    private int currentPoint;

    [Header("Gizmo parameters")]
    [SerializeField] private Color pointColour = Color.red;
    [SerializeField] private float pointSize = 0.1f;

    public Transform CurrentWaypoint()
    {
        return waypoints[currentPoint].transform;
    }

    public void GetNextWaypoint()
    {
        currentPoint++;
        if (currentPoint >= waypoints.Length)
        {
            currentPoint = 0;
        }
    }

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
