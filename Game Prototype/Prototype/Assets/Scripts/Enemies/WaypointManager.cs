using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    [Header("Configuration")]
    public Transform[] waypoints;
    private int currentPoint;

    [Header("Gizmo parameters")]
    public Color pointColour = Color.white;
    public float pointSize = 0.3f;
    public Color lineColour = Color.red;

    public Transform CurrentWaypoint()
    {
        return waypoints[currentPoint];
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
                Gizmos.DrawSphere(waypoints[i].position, pointSize);
            }
            if (waypoints.Length == 1 || (waypoints.Length == (i + 1)))
            {
                Gizmos.color = lineColour;
                Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
                return;
            }

            Gizmos.color = lineColour;
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }
}
