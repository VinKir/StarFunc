using TMPro;
using UnityEngine;

public class FunctionInputBinder : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private FunctionGraphGenerator graphGenerator;

    private void Awake()
    {
        if (inputField != null)
            inputField.onValueChanged.AddListener(OnInputChanged);
    }

    private void OnDestroy()
    {
        if (inputField != null)
            inputField.onValueChanged.RemoveListener(OnInputChanged);
    }

    private void OnInputChanged(string value)
    {
        if (graphGenerator != null)
            graphGenerator.FunctionExpression = value;
    }
}
