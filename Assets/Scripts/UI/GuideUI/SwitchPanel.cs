using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SwitchPanel : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject objectA;
    [SerializeField] private GameObject objectB;

    [Header("Text Output")]
    [SerializeField] private TMP_Text statusText;

    [Header("Text Values")]
    [SerializeField] private string textForObjectA;
    [SerializeField] private string textForObjectB;

    [Header("Button")]
    [SerializeField] private Button toggleButton;


    public TheoryAnimationController animationController;

    private bool isActive = true;

    private void Start()
    {
        if (toggleButton != null)
            toggleButton.onClick.AddListener(OnToggle);

        ApplyState();
    }

    private void OnToggle()
    {
        isActive = !isActive;
        ApplyState();
        animationController.StopAnimation();
    }

    private void ApplyState()
    {
        if (objectA != null) objectA.SetActive(isActive);
        if (objectB != null) objectB.SetActive(!isActive);

        if (statusText != null)
            statusText.text = isActive ? textForObjectA : textForObjectB;
    }
}
