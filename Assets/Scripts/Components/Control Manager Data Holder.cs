#nullable enable

using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(LevelManager))]
public class ControlManagerDataHolder : MonoBehaviour
{
    [Header("General references")]
    public FunctionGraphGenerator? graphGenerator = null;
    [Header("Argument Slider Control Manager")]
    public GameObject? argumentSliderSectionPrefab = null;
    public ArgumentSliderUIPanel? argumentSliderUIPanel = null;
}