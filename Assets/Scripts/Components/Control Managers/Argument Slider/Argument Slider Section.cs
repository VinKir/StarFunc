#nullable enable

using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ArgumentSliderSection : MonoBehaviour
{
    public ArgumentSliderControlManager? argumentSlider = null;
    public int sectionIndex = -1;

    [SerializeField]
    private TextMeshProUGUI? labelMinValue = null;
    [SerializeField]
    private TextMeshProUGUI? labelMaxValue = null;

    private Slider? slider = null;

    public void SetColor(Color color)
    {
        if (slider != null)
        {
            var colors = slider.colors;
            colors.normalColor = color;
            colors.highlightedColor = color;
            colors.pressedColor = new Color(color.r * 0.7f, color.g * 0.7f, color.b * 0.7f);
            colors.selectedColor = color;
            colors.disabledColor = color * 0.5f;
            slider.colors = colors;
        }
    }

    public void SetMaxValue(float maxValue)
    {
        if (slider != null)
        {
            slider.maxValue = maxValue;
        }

        if (labelMaxValue != null)
        {
            labelMaxValue.text = maxValue.ToString("F2");
        }
    }

    public void SetMinValue(float minValue)
    {
        if (slider != null)
        {
            slider.minValue = minValue;
        }

        if (labelMinValue != null)
        {
            labelMinValue.text = minValue.ToString("F2");
        }
    }

    public void UpdateValue(float value)
    {
        argumentSlider?.UpdateValue(this, value);
    }

    public float GetValue() => slider != null ? slider.value : float.NaN;

    public void SetValue(float value)
    {
        if (slider != null)
        {
            slider.value = value;
        }
    }

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }
}
