#nullable enable

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FunctionGraphGenerator))]
class FunctionGraphGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var generator = target as FunctionGraphGenerator;
        if (generator == null) return;

        if (GUILayout.Button("Compute Function Graph"))
        {
            generator.ComputeFunctionGraph(true);
        }

        if (GUILayout.Button("Clear Collision Data"))
        {
            if (generator.TryGetComponent<EdgeCollider2D>(out var edgeCollider))
            {
                Undo.RecordObject(edgeCollider, "Clear Collision Data");

                edgeCollider.Reset();
                edgeCollider.points = new Vector2[] { Vector2.zero, Vector2.right * 0.01f }; // Minimum 2 points

                EditorUtility.SetDirty(edgeCollider);
                EditorUtility.SetDirty(generator);

                // Force scene to save changes
                if (!Application.isPlaying)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(generator.gameObject.scene);
                }
            }
        }
    }
}