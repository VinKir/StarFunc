#nullable enable
using UnityEngine;

public class LevelProgressManager : MonoBehaviour
{
    private static LevelProgressManager? _instance;
    public static LevelProgressManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("LevelProgressManager");
                _instance = go.AddComponent<LevelProgressManager>();
                DontDestroyOnLoad(go);
                _instance.Load();
            }
            return _instance;
        }
    }

    private const string SAVE_KEY = "LevelProgress";
    private const int LEVEL_COUNT = 20;

    public LevelProgressData Data { get; private set; } = new(LEVEL_COUNT);

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            Data = JsonUtility.FromJson<LevelProgressData>(
                PlayerPrefs.GetString(SAVE_KEY)
            );
        }
        else
        {
            Data = new LevelProgressData(LEVEL_COUNT);
        }
    }

    public void Save()
    {
        PlayerPrefs.SetString(SAVE_KEY, JsonUtility.ToJson(Data));
        PlayerPrefs.Save();
    }

    public int GetStars(int levelIndex) => Data.GetStars(levelIndex);

    public void SetStars(int levelIndex, int stars)
    {
        Data.SetStars(levelIndex, stars);
        Save();
    }

    public int GetLastUnlockedLevel()
    {
        for (int i = 0; i < Data.starsPerLevel.Length; i++)
        {
            if (Data.starsPerLevel[i] == 0)
                return i;
        }
        return Data.starsPerLevel.Length - 1;
    }
}
