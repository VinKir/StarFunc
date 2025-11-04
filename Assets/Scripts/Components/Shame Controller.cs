using TMPro;
using UnityEngine;

class ShameController : MonoBehaviour
{
    public TextMeshProUGUI text;
    public FunctionGraphGenerator graphGenerator;

    public void OnSliderValueChanged(float value)
    {
        text.text = "y = <color=\"orange\"><u><b>" + value.ToString("F2") + "</b></u></color>x";

        if (graphGenerator != null)
        {
            graphGenerator.FunctionExpression = $"{value} * x";
        }
    }
}