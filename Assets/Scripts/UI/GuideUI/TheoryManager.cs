using UnityEngine;
using System.Linq;

public class TheoryManager : MonoBehaviour
{
    public TheoryTopic[] topics;
    public TheoryUI ui;
    public TheoryAnimationController animationController;

    private int currentIndex = 0;
    private TheoryTopic currentTopic;

    void Start()
    {
        currentTopic = topics.FirstOrDefault(t => t.topicName == SceneData.SelectedTopic);
        if (currentTopic == null)
        {
            Debug.LogWarning("Тема не найдена! Загружается первая по умолчанию.");
            currentTopic = topics.FirstOrDefault();
        }

        ui.Setup(currentTopic.parts.Length, ShowPart);
        ui.BindButtons(NextPart, PrevPart);
        ShowPart(0);
    }

    public void ShowPart(int index)
    {
        if (currentTopic == null || index < 0 || index >= currentTopic.parts.Length)
            return;

        currentIndex = index;
        var part = currentTopic.parts[index];

        ui.UpdateText(part.description, index);
        animationController.PlayAnimation(part.animationType);
    }

    public void NextPart()
    {
        if (currentIndex < currentTopic.parts.Length - 1)
            ShowPart(currentIndex + 1);
    }

    public void PrevPart()
    {
        if (currentIndex > 0)
            ShowPart(currentIndex - 1);
    }
}
