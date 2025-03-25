using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class AILocationMap : MonoBehaviour
{
    [System.Serializable]
    public class PatrolArea
    {
        public string areaName;
        public Vector3 centerPoint;
        public float radius = 10f;
        public Color gizmoColor = Color.blue;
    }

    [Header("Movement Areas")]
    [SerializeField] private List<PatrolArea> patrolAreas = new List<PatrolArea>();
    [SerializeField] private Color connectionColor = Color.cyan;

    public int GetAreaCount()
    {
        return patrolAreas.Count;
    }

    public PatrolArea GetArea(int index)
    {
        if (index < 0 || index >= patrolAreas.Count)
        {
            Debug.LogWarning("Area index out of range");
            return null;
        }
        return patrolAreas[index];
    }

    public PatrolArea GetAreaByName(string name)
    {
        return patrolAreas.Find(area => area.areaName == name);
    }

    public Vector3 GetRandomPointInArea(int areaIndex)
    {
        if (areaIndex < 0 || areaIndex >= patrolAreas.Count)
        {
            Debug.LogWarning("Area index out of range");
            return transform.position;
        }

        PatrolArea area = patrolAreas[areaIndex];
        Vector2 randomCircle = Random.insideUnitCircle * area.radius;
        return area.centerPoint + new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    public Vector3 GetRandomPointInArea(string areaName)
    {
        PatrolArea area = GetAreaByName(areaName);
        if (area == null)
        {
            Debug.LogWarning($"Area '{areaName}' not found");
            return transform.position;
        }

        Vector2 randomCircle = Random.insideUnitCircle * area.radius;
        return area.centerPoint + new Vector3(randomCircle.x, 0, randomCircle.y);
    }

    // Editor methods to help set up areas
    public void AddArea(string name, Vector3 position, float radius)
    {
        PatrolArea newArea = new PatrolArea
        {
            areaName = name,
            centerPoint = position,
            radius = radius,
            gizmoColor = new Color(Random.value, Random.value, Random.value)
        };
        patrolAreas.Add(newArea);
    }

    public void RemoveArea(int index)
    {
        if (index >= 0 && index < patrolAreas.Count)
        {
            patrolAreas.RemoveAt(index);
        }
    }

    private void OnDrawGizmos()
    {
        foreach (PatrolArea area in patrolAreas)
        {
            Gizmos.color = area.gizmoColor;
            Gizmos.DrawWireSphere(area.centerPoint, area.radius);

            // Draw area name
            Handles.Label(area.centerPoint, area.areaName);
        }
    }
}
