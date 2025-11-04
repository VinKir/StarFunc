#nullable enable

using UnityEngine;

class ArgumentSlider : IControlManager
{
    public float argumentMin = -10f;
    public float argumentMax = 10f;

    // argument slider is prefab for instancing an actual slider in the scene
    public GameObject? argumentSlider;
    public FunctionGraphGenerator? graphGenerator;

    public ArgumentSlider(ControlManagerDataHolder dataHolder)
    {
        argumentSlider = dataHolder.argumentSlider;
        graphGenerator = dataHolder.graphGenerator;
    }

    public void Reset()
    {
    }
}