#nullable enable
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectInitializer : MonoBehaviour
{
    [Header("Level Groups")]
    [SerializeField] private Transform levels0Stars = null!;
    [SerializeField] private Transform levels1Stars = null!;
    [SerializeField] private Transform levels2Stars = null!;
    [SerializeField] private Transform levels3Stars = null!;

    [SerializeField] private List<LevelDefinition> levelDefinitions;

    private Transform[] groups;

    private void Awake()
    {
        groups = new[]
        {
            levels0Stars,
            levels1Stars,
            levels2Stars,
            levels3Stars
        };

        Initialize();
    }

    private void Initialize()
    {
        var progress = LevelProgressManager.Instance;
        int lastUnlocked = progress.GetLastUnlockedLevel();

        foreach (var group in groups)
        {
            foreach (Transform lvl in group)
                lvl.gameObject.SetActive(false);
        }

        for (int i = 0; i < 20; i++)
        {
            int stars = progress.GetStars(i);

            Transform targetGroup = groups[Mathf.Clamp(stars, 0, 3)];
            Transform levelButton = targetGroup.GetChild(i);

            levelButton.gameObject.SetActive(true);
            var button = levelButton.GetComponent<Button>();

            var levelDef = levelDefinitions[0];
            if (i >= levelDefinitions.Count)
                Debug.LogAssertion("Не хватает уровней! Доделать уровни и вставить их все в поле levelDefinitions!");
            else
                levelDef = levelDefinitions[i];

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => LoadLevel(levelDef));

            var lockObj = levelButton.Find("LevelClosed");
            if (lockObj != null)
            {
                bool locked = i > lastUnlocked;
                lockObj.gameObject.SetActive(locked);
                button.interactable = !locked;
            }
        }
    }

    public void LoadLevel(LevelDefinition level)
    {
        LevelProgressManager.Instance.SelectedLevel = level;

        SceneManager.LoadScene("Scenes/Level");
    }
}
