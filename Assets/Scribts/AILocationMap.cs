using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AILocationMap))]
public class AILocationMapEditor : Editor
{
    private string newAreaName = "New Area";
    private float newAreaRadius = 10f;

    public override void OnInspectorGUI()
    {
        AILocationMap locationMap = (AILocationMap)target;

        // Draw default inspector
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Add New Area", EditorStyles.boldLabel);

        // Area name field
        newAreaName = EditorGUILayout.TextField("Area Name", newAreaName);

        // Area radius field
        newAreaRadius = EditorGUILayout.FloatField("Area Radius", newAreaRadius);

        // Add area button
        if (GUILayout.Button("Add Area at Scene View Camera"))
        {
            // Get scene view camera position
            Vector3 cameraPosition = SceneView.lastActiveSceneView.camera.transform.position;
            cameraPosition.y = 0; // Set to ground level

            // Add new area
            locationMap.AddArea(newAreaName, cameraPosition, newAreaRadius);

            // Reset name field with incremented number
            int areaNumber = 1;
            if (newAreaName.Contains(" "))
            {
                string[] parts = newAreaName.Split(' ');
                if (int.TryParse(parts[parts.Length - 1], out int num))
                {
                    areaNumber = num + 1;
                    newAreaName = string.Join(" ", parts, 0, parts.Length - 1) + " " + areaNumber;
                }
                else
                {
                    newAreaName += " " + areaNumber;
                }
            }
            else
            {
                newAreaName += " " + areaNumber;
            }

            // Mark the object as dirty
            EditorUtility.SetDirty(locationMap);
        }
    }

    private void OnSceneGUI()
    {
        AILocationMap locationMap = (AILocationMap)target;

        for (int i = 0; i < locationMap.GetAreaCount(); i++)
        {
            AILocationMap.PatrolArea area = locationMap.GetArea(i);
            if (area == null) continue;

            // Make the center point movable
            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.PositionHandle(area.centerPoint, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(locationMap, "Move Area Center");
                area.centerPoint = newPos;
                EditorUtility.SetDirty(locationMap);
            }

            // Make the radius adjustable
            EditorGUI.BeginChangeCheck();
            float newRadius = Handles.RadiusHandle(Quaternion.identity, area.centerPoint, area.radius);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(locationMap, "Change Area Radius");
                area.radius = newRadius;
                EditorUtility.SetDirty(locationMap);
            }
        }
    }
}
