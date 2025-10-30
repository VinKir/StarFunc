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
            generator.ComputeFunctionGraph(() => { });
        }
    }
}