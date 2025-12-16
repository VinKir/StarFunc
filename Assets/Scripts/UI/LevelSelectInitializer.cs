#nullable enable
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectInitializer : MonoBehaviour
{
    [Header("Level Groups")]
    [SerializeField] private Transform levels0Stars = null!;
    [SerializeField] private Transform levels1Stars = null!;
    [SerializeField] private Transform levels2Stars = null!;
    [SerializeField] private Transform levels3Stars = null!;

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

            var lockObj = levelButton.Find("LevelClosed");
            if (lockObj != null)
            {
                bool locked = i > lastUnlocked;
                lockObj.gameObject.SetActive(locked);
            }
        }
    }
}
