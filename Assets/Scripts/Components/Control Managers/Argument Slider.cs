#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

public class ArgumentSliderControlManager : IControlManager
{
    public ControlManagerDataHolder dataHolder;
    public LevelDefinition levelDefinition;

    private readonly List<ValueTuple<ArgumentSliderSection, float>> sliderSections = new();

    public ArgumentSliderControlManager(LevelDefinition levelDefinition, ControlManagerDataHolder dataHolder)
    {
        this.levelDefinition = levelDefinition;
        this.dataHolder = dataHolder;

        if (dataHolder.argumentSliderUIPanel == null)
        {
            Debug.LogError("Argument Slider UI Panel is not assigned in Control Manager Data Holder.");
            return;
        }

        dataHolder.argumentSliderUIPanel.Show();

        if (dataHolder.argumentSliderSectionPrefab == null)
        {
            Debug.LogError("Argument Slider Section Prefab is not assigned in Control Manager Data Holder.");
            return;
        }

        Transform? slidersContainer = dataHolder.argumentSliderUIPanel != null ? dataHolder.argumentSliderUIPanel.SlidersContainer : null;

        foreach (var sliderSectionSettings in levelDefinition.argumentSliderSettings)
        {
            var sliderSection = UnityEngine.Object.Instantiate(
                dataHolder.argumentSliderSectionPrefab,
                slidersContainer
            ).GetComponent<ArgumentSliderSection>();

            sliderSection.SetColor(sliderSectionSettings.color);
            sliderSection.SetMinValue(sliderSectionSettings.minValue);
            sliderSection.SetMaxValue(sliderSectionSettings.maxValue);
            sliderSection.SetValue(sliderSectionSettings.initialValue);
            sliderSection.argumentSlider = this;
            sliderSection.sectionIndex = sliderSections.Count;
            sliderSection.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -80f * (sliderSection.sectionIndex + 1));
            sliderSections.Add(new ValueTuple<ArgumentSliderSection, float>(sliderSection, sliderSectionSettings.initialValue));
        }

        UpdateFormula();
    }

    public void Reset()
    {
    }

    public void UpdateValue(ArgumentSliderSection sliderSection, float value)
    {
        var index = sliderSection.sectionIndex;
        var tuple = sliderSections[index];
        sliderSections[index] = (tuple.Item1, value);

        UpdateFormula();
    }

    private void UpdateFormula()
    {
        if (dataHolder.argumentSliderUIPanel == null ||
            dataHolder.argumentSliderUIPanel.FunctionLabel == null ||
            dataHolder.graphGenerator == null)
        {
            return;
        }

        var formulaTemplate = levelDefinition.originalFunctionTemplate;
        var actualFormula = formulaTemplate;
        var printableFormula = "y = " + formulaTemplate;

        for (int i = 0; i < sliderSections.Count; i++)
        {
            var sliderSection = sliderSections[i];
            var sliderValue = sliderSection.Item2;
            actualFormula = actualFormula.Replace($"{{{i}}}", sliderValue.ToString("F2"));
            printableFormula = printableFormula.Replace($"{{{i}}}", $"<color=#{ColorUtility.ToHtmlStringRGB(levelDefinition.argumentSliderSettings[i].color)}><u><b>{sliderValue:F2}</b></u></color>");
        }

        dataHolder.graphGenerator.FunctionExpression = actualFormula;
        dataHolder.argumentSliderUIPanel.FunctionLabel.text = printableFormula;
    }
}