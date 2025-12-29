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

    [Serializable]
    public struct FunctionSelectorSectionSettings
    {
        public string function;
        public Color color;
    }

    public int levelIndex = 0; // start index - 0

    public string levelFunction = "sin(x)";
    public float maxRunningSeconds = 60f;
    public Vector2 circlePosition = new(0f, 0f);
    public Vector2 cameraOffset = new(0f, 0f);
    public Vector2[] starPositions = Array.Empty<Vector2>();
    public LevelManager.ControlManagers controlManager = LevelManager.ControlManagers.ArgumentSlider;

    [Header("Argument Slider Control Manager Settings")]
    public string originalFunctionTemplate = "sin({0}*x)";
    public ArgumentSliderSectionSettings[] argumentSliderSettings = Array.Empty<ArgumentSliderSectionSettings>();

    [Header("Function Selector Control Manager Settings")]
    public FunctionSelectorSectionSettings[] functionSelectorSettings = Array.Empty<FunctionSelectorSectionSettings>();

    [Header("Background (Sprite PNG)")]
    public Sprite backgroundSprite;

    private void OnValidate()
    {
        int arguments = originalFunctionTemplate.Split(new char[] { '{' }).Length - 1;

        if (argumentSliderSettings.Length != arguments)
        {
            Array.Resize(ref argumentSliderSettings, arguments);
        }
    }
}