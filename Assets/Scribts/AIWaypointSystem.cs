using System.Collections.Generic;
using UnityEngine;

public class AIWaypointSystem : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    [SerializeField] private bool isLoop = true;
    [SerializeField] private Color waypointColor = Color.blue;
    [SerializeField] private Color pathColor = Color.cyan;
    [SerializeField] private float waypointSize = 0.3f;

    public int GetWaypointCount()
    {
        return waypoints.Count;
    }

    public Transform GetWaypoint(int index)
    {
        if (waypoints.Count == 0) return null;

        if (index < 0 || index >= waypoints.Count)
        {
            Debug.LogWarning("Waypoint index out of range");
            return null;
        }

        return waypoints[index];
    }

    public Transform GetNextWaypoint(int currentIndex)
    {
        if (waypoints.Count == 0) return null;

        int nextIndex = currentIndex + 1;

        // If we reached the end
        if (nextIndex >= waypoints.Count)
        {
            if (isLoop)
            {
                nextIndex = 0; // Loop back to start
            }
            else
            {
                nextIndex = waypoints.Count - 1; // Stay at last waypoint
            }
        }

        return waypoints[nextIndex];
    }

    private void OnDrawGizmos()
    {
        if (waypoints.Count == 0) return;

        // Draw waypoints
        Gizmos.color = waypointColor;
        foreach (Transform waypoint in waypoints)
        {
            if (waypoint != null)
                Gizmos.DrawSphere(waypoint.position, waypointSize);
        }

        // Draw path between waypoints
        Gizmos.color = pathColor;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }

        // Draw line from last to first waypoint if it's a loop
        if (isLoop && waypoints.Count > 1)
        {
            if (waypoints[0] != null && waypoints[waypoints.Count - 1] != null)
                Gizmos.DrawLine(waypoints[waypoints.Count - 1].position, waypoints[0].position);
        }
    }

    // Helper method to add waypoints in editor
    public void AddWaypoint(Transform waypoint)
    {
        if (waypoint != null && !waypoints.Contains(waypoint))
        {
            waypoints.Add(waypoint);
        }
    }

    // Helper method to clear all waypoints
    public void ClearWaypoints()
    {
        waypoints.Clear();
    }
}
