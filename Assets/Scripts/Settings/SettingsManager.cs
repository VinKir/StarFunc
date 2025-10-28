#nullable enable

using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private static SettingsManager? _instance;
    public static SettingsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject settingsObject = new("SettingsManager");
                _instance = settingsObject.AddComponent<SettingsManager>();
                DontDestroyOnLoad(settingsObject);
                _instance.LoadSettings();
            }
            return _instance;
        }
    }

    private const string SETTINGS_KEY = "GameSettings";

    public GameSettings CurrentSettings { get; private set; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSettings();
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey(SETTINGS_KEY))
        {
            string json = PlayerPrefs.GetString(SETTINGS_KEY);
            CurrentSettings = JsonUtility.FromJson<GameSettings>(json);
        }
        else
        {
            CurrentSettings = GameSettings.Default();
        }
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(CurrentSettings);
        PlayerPrefs.SetString(SETTINGS_KEY, json);
        PlayerPrefs.Save();
    }

    public void UpdateSettings(GameSettings newSettings)
    {
        CurrentSettings = newSettings;
        SaveSettings();
    }

    public void SetMusicEnabled(bool enabled)
    {
        GameSettings settings = CurrentSettings;
        settings.isMusicOn = enabled;
        UpdateSettings(settings);
    }

    public void SetSoundEffectsEnabled(bool enabled)
    {
        GameSettings settings = CurrentSettings;
        settings.isSoundEffectsOn = enabled;
        UpdateSettings(settings);
    }

    public void ResetToDefaults()
    {
        UpdateSettings(GameSettings.Default());
    }
}
