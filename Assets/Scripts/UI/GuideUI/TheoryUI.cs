using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class TheoryUI : MonoBehaviour
{
    public TextMeshProUGUI descriptionText;
    public Transform dotsContainer;
    public GameObject dotPrefab;
    public Button nextButton;
    public Button prevButton;
    public TheoryManager theoryManager;

    private Image[] dots;

    public void Setup(int count, Action<int> onSelectPartCallback)
    {
        foreach (Transform child in dotsContainer)
            Destroy(child.gameObject);

        dots = new Image[count];
        for (int i = 0; i < count; i++)
        {
            var dot = Instantiate(dotPrefab, dotsContainer);
            dots[i] = dot.GetComponent<Image>();
        }
    }

    public void BindButtons(Action onNext, Action onPrev)
    {
        nextButton.onClick.RemoveAllListeners();
        prevButton.onClick.RemoveAllListeners();

        nextButton.onClick.AddListener(() => onNext?.Invoke());
        prevButton.onClick.AddListener(() => onPrev?.Invoke());
    }

    public void UpdateText(string text, int activeIndex)
    {
        descriptionText.text = text;

        for (int i = 0; i < dots.Length; i++)
            dots[i].color = i == activeIndex ? Color.white : new Color(1f, 1f, 1f, 0.3f);
    }
}