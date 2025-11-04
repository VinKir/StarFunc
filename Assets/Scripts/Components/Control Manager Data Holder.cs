#nullable enable

using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(LevelManager))]
class ControlManagerDataHolder : MonoBehaviour
{
    [Header("General references")]
    public FunctionGraphGenerator? graphGenerator = null;
    [Header("Argument Slider Control Manager")]
    public GameObject? argumentSlider = null;
    public float argumentMin = -10f;
    public float argumentMax = 10f;
}