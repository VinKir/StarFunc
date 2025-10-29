#nullable enable

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Менеджер пользовательского интерфейса.
/// Управляет панелями, кнопками и музыкой в игре.
/// </summary>
public class UIManager : MonoBehaviour
{
    #region Public Fields

    [Header("Panels")]
    public GameObject? mainMenuPanel; // Панель главного меню

    [Header("Music Control")]
    public GameObject? buttonMusic; // Кнопка управления музыкой
    public Sprite? musicOnIcon; // Иконка включённой музыки
    public Sprite? musicOffIcon; // Иконка выключенной музыки
    public AudioSource? musicPlayer; // Аудио источник для воспроизведения музыки

    [Header("Buttons (Main Menu)")]
    public Button? playButton; // Кнопка "Играть"
    public Button? settingsButton; // Кнопка настроек
    public Button? guideButton; // Кнопка руководства/гайда
    public Button? shopButton; // Кнопка магазина

    [Header("Buttons (Settings Panel)")]
    public Button? aboutButtonButton; // Кнопка "О приложении"

    #endregion

    #region Private Fields

    private GameObject? currentPanel = null; // Текущая активная панель

    #endregion

    #region Public Methods

    /// <summary>
    /// Переключает состояние музыки (вкл/выкл).
    /// </summary>
    public void ToggleMusic()
    {
        // Проверяем текущее состояние музыки и переключаем её
        if (SettingsManager.Instance.CurrentSettings.isMusicOn)
        {
            TurnMusicOff();
        }
        else
        {
            TurnMusicOn();
        }
    }

    /// <summary>
    /// Включает музыку и обновляет иконку кнопки.
    /// </summary>
    public void TurnMusicOn()
    {
        // Проверяем, что все необходимые компоненты назначены
        if (musicPlayer == null || buttonMusic == null || musicOnIcon == null)
        {
            return;
        }

        // Устанавливаем громкость на максимум
        musicPlayer.volume = 1f;
        // Меняем иконку на "музыка включена"
        buttonMusic.GetComponent<Button>().image.sprite = musicOnIcon;
        // Сохраняем состояние в настройках
        SettingsManager.Instance.SetMusicEnabled(true);
    }

    /// <summary>
    /// Выключает музыку и обновляет иконку кнопки.
    /// </summary>
    public void TurnMusicOff()
    {
        // Проверяем, что все необходимые компоненты назначены
        if (musicPlayer == null || buttonMusic == null || musicOffIcon == null)
        {
            return;
        }

        // Устанавливаем громкость на ноль
        musicPlayer.volume = 0f;
        // Меняем иконку на "музыка выключена"
        buttonMusic.GetComponent<Button>().image.sprite = musicOffIcon;
        // Сохраняем состояние в настройках
        SettingsManager.Instance.SetMusicEnabled(false);
    }

    /// <summary>
    /// Показывает указанную панель, скрывая текущую активную.
    /// </summary>
    /// <param name="panel">Панель для отображения</param>
    public void ShowPanel(GameObject panel)
    {
        // Если текущая панель не назначена или это та же панель, ничего не делаем
        if (currentPanel == null || currentPanel == panel)
        {
            return;
        }

        // Скрываем текущую панель
        currentPanel.SetActive(false);
        // Устанавливаем новую панель как текущую
        currentPanel = panel;
        // Показываем новую панель
        currentPanel.SetActive(true);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Инициализация при пробуждении объекта.
    /// Устанавливает состояние музыки согласно сохранённым настройкам.
    /// </summary>
    private void Awake()
    {
        // Загружаем состояние музыки из настроек и применяем его
        if (SettingsManager.Instance.CurrentSettings.isMusicOn)
        {
            TurnMusicOn();
        }
        else
        {
            TurnMusicOff();
        }
    }

    /// <summary>
    /// Инициализация при старте.
    /// Устанавливает главное меню как активную панель и подготавливает обработчики событий.
    /// </summary>
    private void Start()
    {
        // Устанавливаем главное меню как текущую панель
        currentPanel = mainMenuPanel;

        // Проверяем, что панель главного меню назначена
        if (currentPanel == null)
        {
            Debug.LogError("UIManager: Main Menu Panel is not assigned.");
            return;
        }
    }

    #endregion
}