#nullable enable

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Definition", menuName = "Scriptable Objects/Level Definition", order = 1)]
public class LevelDefinition : ScriptableObject
{
    [Serializable]
    public struct ArgumentSliderSectionSettings
    {
        public float minValue;
        public float maxValue;
        public float initialValue;
        public Color color;
    }

    public string levelFunction = "sin(x)";
    public Vector2 circlePosition = new(0f, 0f);
    public Vector2[] starPositions = Array.Empty<Vector2>();
    public LevelManager.ControlManagers controlManager = LevelManager.ControlManagers.ArgumentSlider;
    public string originalFunctionTemplate = "sin({0}*x)";

    // Argument Slider specific

    [Header("Argument Slider Control Manager Settings")]
    public ArgumentSliderSectionSettings[] argumentSliderSettings = Array.Empty<ArgumentSliderSectionSettings>();

    private void OnValidate()
    {
        int arguments = originalFunctionTemplate.Split(new char[] { '{' }).Length - 1;

        if (argumentSliderSettings.Length != arguments)
        {
            Array.Resize(ref argumentSliderSettings, arguments);
        }
    }
}