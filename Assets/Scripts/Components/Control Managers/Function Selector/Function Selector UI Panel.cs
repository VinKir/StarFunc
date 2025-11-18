#nullable enable

using UnityEngine;

public class FunctionSelectorUIPanel : MonoBehaviour
{
    [field: SerializeField]
    public Transform? ButtonsContainer { get; private set; }

    [field: SerializeField]
    public GameObject? FunctionButtonPrefab { get; private set; }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}