using System.Collections.Generic;
using UnityEngine;

public class AIWaypointSystem : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    [SerializeField] private Color waypointColor = Color.green;
    [SerializeField] private Color connectionColor = Color.yellow;
    [SerializeField] private float waypointSize = 0.5f;

    public int GetWaypointCount()
    {
        return waypoints.Count;
    }

    public Transform GetWaypoint(int index)
    {
        if (index < 0 || index >= waypoints.Count)
        {
            Debug.LogWarning("Waypoint index out of range");
            return null;
        }

        return waypoints[index];
    }

    public void AddWaypoint(Transform waypoint)
    {
        if (waypoint != null && !waypoints.Contains(waypoint))
        {
            waypoints.Add(waypoint);
        }
    }

    public void RemoveWaypoint(int index)
    {
        if (index >= 0 && index < waypoints.Count)
        {
            waypoints.RemoveAt(index);
        }
    }

    public void ClearWaypoints()
    {
        waypoints.Clear();
    }

    private void OnDrawGizmos()
    {
        if (waypoints.Count == 0) return;

        // Draw waypoints
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == null) continue;

            // Draw waypoint
            Gizmos.color = waypointColor;
            Gizmos.DrawSphere(waypoints[i].position, waypointSize);

            // Draw connection to next waypoint
            Gizmos.color = connectionColor;
            int nextIndex = (i + 1) % waypoints.Count;
            if (waypoints[nextIndex] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[nextIndex].position);
            }

            // Draw waypoint number
            UnityEditor.Handles.Label(waypoints[i].position + Vector3.up * waypointSize, $"Waypoint {i}");
        }
    }
}
