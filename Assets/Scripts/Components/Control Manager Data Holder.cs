#nullable enable

using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(LevelManager))]
public class ControlManagerDataHolder : MonoBehaviour
{
    [Header("General references")]
    public FunctionGraphGenerator? graphGenerator = null;
    public ArgumentSliderUIPanel? argumentSliderUIPanel = null;
    public FunctionSelectorUIPanel? functionSelectorUIPanel = null;
}