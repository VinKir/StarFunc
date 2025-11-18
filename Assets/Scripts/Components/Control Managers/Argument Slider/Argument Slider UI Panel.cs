#nullable enable

using UnityEngine;
using TMPro;

public class ArgumentSliderUIPanel : MonoBehaviour
{
    [field: SerializeField]
    public Transform? SlidersContainer { get; private set; }

    [field: SerializeField]
    public TextMeshProUGUI? FunctionLabel { get; private set; }

    [field: SerializeField]
    public GameObject? ArgumentSliderPrefab { get; private set; }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
