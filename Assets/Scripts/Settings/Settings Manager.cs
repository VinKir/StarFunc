#nullable enable

using UnityEngine;

/// <summary>
/// Менеджер настроек игры.
/// Реализует паттерн Singleton для глобального доступа к настройкам.
/// Обеспечивает загрузку, сохранение и управление игровыми настройками.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    /// <summary>
    /// Приватное поле для хранения единственного экземпляра менеджера.
    /// </summary>
    private static SettingsManager? _instance;

    /// <summary>
    /// Публичный доступ к единственному экземпляру менеджера настроек.
    /// Создаёт экземпляр при первом обращении, если он ещё не существует.
    /// </summary>
    public static SettingsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Создаём новый GameObject для менеджера настроек
                GameObject settingsObject = new("SettingsManager");
                _instance = settingsObject.AddComponent<SettingsManager>();
                // Сохраняем объект между сценами
                DontDestroyOnLoad(settingsObject);
                _instance.LoadSettings();
            }
            return _instance;
        }
    }

    /// <summary>
    /// Ключ для сохранения и загрузки настроек из PlayerPrefs.
    /// </summary>
    private const string SETTINGS_KEY = "GameSettings";

    /// <summary>
    /// Текущие активные настройки игры.
    /// </summary>
    public GameSettings CurrentSettings { get; private set; }

    /// <summary>
    /// Инициализация при пробуждении объекта.
    /// Гарантирует, что существует только один экземпляр менеджера.
    /// </summary>
    private void Awake()
    {
        // Если экземпляр уже существует и это не он, уничтожаем дубликат
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Устанавливаем этот экземпляр как единственный
        _instance = this;
        // Сохраняем объект между загрузками сцен
        DontDestroyOnLoad(gameObject);
        LoadSettings();
    }

    /// <summary>
    /// Загружает настройки из PlayerPrefs.
    /// Если сохранённых настроек нет, использует настройки по умолчанию.
    /// </summary>
    public void LoadSettings()
    {
        // Проверяем, есть ли сохранённые настройки
        if (PlayerPrefs.HasKey(SETTINGS_KEY))
        {
            // Загружаем JSON-строку из PlayerPrefs
            string json = PlayerPrefs.GetString(SETTINGS_KEY);
            // Десериализуем JSON в структуру GameSettings
            CurrentSettings = JsonUtility.FromJson<GameSettings>(json);
        }
        else
        {
            // Если сохранений нет, используем настройки по умолчанию
            CurrentSettings = GameSettings.Default();
        }
    }

    /// <summary>
    /// Сохраняет текущие настройки в PlayerPrefs.
    /// </summary>
    public void SaveSettings()
    {
        // Сериализуем настройки в JSON
        string json = JsonUtility.ToJson(CurrentSettings);
        // Сохраняем в PlayerPrefs
        PlayerPrefs.SetString(SETTINGS_KEY, json);
        // Принудительно записываем изменения на диск
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Обновляет настройки и сохраняет их.
    /// </summary>
    /// <param name="newSettings">Новые настройки для применения.</param>
    public void UpdateSettings(GameSettings newSettings)
    {
        CurrentSettings = newSettings;
        SaveSettings();
    }

    /// <summary>
    /// Устанавливает состояние музыки (включена/выключена).
    /// </summary>
    /// <param name="enabled">True - включить музыку, False - выключить.</param>
    public void SetMusicEnabled(bool enabled)
    {
        // Создаём копию текущих настроек
        GameSettings settings = CurrentSettings;
        // Изменяем настройку музыки
        settings.isMusicOn = enabled;
        // Обновляем и сохраняем настройки
        UpdateSettings(settings);
    }

    /// <summary>
    /// Устанавливает состояние звуковых эффектов (включены/выключены).
    /// </summary>
    /// <param name="enabled">True - включить звуковые эффекты, False - выключить.</param>
    public void SetSoundEffectsEnabled(bool enabled)
    {
        // Создаём копию текущих настроек
        GameSettings settings = CurrentSettings;
        // Изменяем настройку звуковых эффектов
        settings.isSoundEffectsOn = enabled;
        // Обновляем и сохраняем настройки
        UpdateSettings(settings);
    }

    /// <summary>
    /// Сбрасывает все настройки к значениям по умолчанию.
    /// </summary>
    public void ResetToDefaults()
    {
        UpdateSettings(GameSettings.Default());
    }
}
