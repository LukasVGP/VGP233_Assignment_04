using UnityEngine;

public class LocationMap : MonoBehaviour
{
    [SerializeField] private float radius = 10f;
    [SerializeField] private Color gizmoColor = Color.cyan;
    [SerializeField] private bool showGizmo = true;
    [SerializeField] private Transform centerPoint;

    private void Awake()
    {
        if (centerPoint == null)
        {
            centerPoint = transform;
        }
    }

    public Vector3 GetCenterPoint()
    {
        return centerPoint != null ? centerPoint.position : transform.position;
    }

    public float GetRadius()
    {
        return radius;
    }

    public Vector3 GetRandomPointInArea()
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        Vector3 randomPoint = GetCenterPoint() + new Vector3(randomCircle.x, 0, randomCircle.y);

        // Make sure the point is on the ground
        if (Physics.Raycast(randomPoint + Vector3.up * 10, Vector3.down, out RaycastHit hit, 20))
        {
            return hit.point;
        }

        return randomPoint;
    }

    private void OnDrawGizmos()
    {
        if (!showGizmo) return;

        Vector3 center = Application.isPlaying ? GetCenterPoint() : (centerPoint != null ? centerPoint.position : transform.position);

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(center, radius);
    }
}
