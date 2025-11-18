#nullable enable

using System.Collections.Generic;
using UnityEngine;

public class FunctionSelectorControlManager : IControlManager
{
    public ControlManagerDataHolder dataHolder;
    public LevelDefinition levelDefinition;

    private readonly List<FunctionSelectorSection> functionSections = new();

    private string currentFormula = "0";

    public FunctionSelectorControlManager(LevelDefinition levelDefinition, ControlManagerDataHolder dataHolder)
    {
        this.levelDefinition = levelDefinition;
        this.dataHolder = dataHolder;

        if (dataHolder.functionSelectorUIPanel == null)
        {
            Debug.LogError("Function Selector UI Panel is not assigned in Control Manager Data Holder.");
            return;
        }

        dataHolder.functionSelectorUIPanel.Show();
        if (dataHolder.argumentSliderUIPanel != null)
        {
            dataHolder.argumentSliderUIPanel.Hide();
        }

        var buttonPrefab = dataHolder.functionSelectorUIPanel.FunctionButtonPrefab;

        if (buttonPrefab == null)
        {
            Debug.LogError("Function Button Prefab is not assigned in Control Manager Data Holder.");
            return;
        }

        Transform? buttonsContainer = dataHolder.functionSelectorUIPanel != null ? dataHolder.functionSelectorUIPanel.ButtonsContainer : null;

        foreach (var functionDefinition in levelDefinition.functionSelectorSettings)
        {
            var functionButton = Object.Instantiate(
                buttonPrefab,
                buttonsContainer
            ).GetComponent<FunctionSelectorSection>();

            functionButton.SetColor(functionDefinition.color);
            functionButton.SetFunctionText(functionDefinition.function);
            functionButton.functionSelector = this;

            functionButton.sectionIndex = functionSections.Count;
            functionSections.Add(functionButton);

            functionButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -120f * (functionButton.sectionIndex + 1));
        }

        UpdateFormula();
    }

    public void Reset()
    {
    }

    public void Click(FunctionSelectorSection selectorSection)
    {
        currentFormula = selectorSection.ActualFormula;
        UpdateFormula();
    }

    public void UpdateFormula()
    {
        if (dataHolder.graphGenerator == null)
        {
            return;
        }

        dataHolder.graphGenerator.FunctionExpression = currentFormula;
    }
}
