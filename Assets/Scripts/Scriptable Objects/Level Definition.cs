#nullable enable

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Definition", menuName = "Scriptable Objects/Level Definition", order = 1)]
class LevelDefinition : ScriptableObject
{
    public string levelFunction = "sin(x)";
    public Vector2 circlePosition = new(0f, 0f);
    public Vector2[] starPositions = Array.Empty<Vector2>();
}