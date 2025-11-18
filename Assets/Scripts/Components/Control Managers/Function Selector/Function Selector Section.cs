#nullable enable

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FunctionSelectorSection : MonoBehaviour
{
    public FunctionSelectorControlManager? functionSelector = null;
    public int sectionIndex = -1;

    [field: SerializeField]
    public TextMeshProUGUI? FunctionText { get; private set; }

    public string ActualFormula { get; private set; } = "";

    public void UpdateFunction()
    {
        functionSelector?.Click(this);
    }

    public void SetFunctionText(string functionText)
    {
        if (FunctionText != null)
        {
            FunctionText.text = "y = " + functionText;
        }
        ActualFormula = functionText;
    }

    public void SetColor(Color color)
    {
        if (TryGetComponent<Image>(out var image))
        {
            image.color = color;
        }

        var lightness = (color.r + color.g + color.b) / 3f;
        if (FunctionText != null)
        {
            FunctionText.color = lightness < 0.5f ? Color.white : Color.black;
        }
    }
}