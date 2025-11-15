using UnityEngine;
using UnityEngine.SceneManagement;

public class GuideMenuButton : MonoBehaviour
{
    public string topicName; // "Functions", "Trigonometry", "Integrals"

    public void OnButtonClick()
    {
        SceneData.SelectedTopic = topicName;
        SceneManager.LoadScene("Scenes/Theory");
    }
}
