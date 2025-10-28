using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject canvasMainMenuPanel;
    public GameObject canvasSettingsPanel;
    public GameObject canvasGuidePanel;

    [Header("Music Control")]
    public GameObject buttonMusicOn;
    public GameObject buttonMusicOff;
    public AudioSource musicPlayer;

    [Header("Buttons")]
    public Button settingsButton;
    public Button guideButton;

    void Start()
    {
        // Кнопки перехода между панелями
        settingsButton.onClick.AddListener(OpenSettings);
        guideButton.onClick.AddListener(OpenGuide);

        // Кнопки управления музыкой
        buttonMusicOn.GetComponent<Button>().onClick.AddListener(TurnMusicOff);
        buttonMusicOff.GetComponent<Button>().onClick.AddListener(TurnMusicOn);

        // Запуск с главного меню
        OpenMainMenu();
    }

    // --- Методы перехода ---
    public void OpenSettings()
    {
        canvasMainMenuPanel.SetActive(false);
        canvasSettingsPanel.SetActive(true);
        canvasGuidePanel.SetActive(false);
    }

    public void OpenGuide()
    {
        canvasMainMenuPanel.SetActive(false);
        canvasSettingsPanel.SetActive(false);
        canvasGuidePanel.SetActive(true);
    }

    public void OpenMainMenu()
    {
        canvasMainMenuPanel.SetActive(true);
        canvasSettingsPanel.SetActive(false);
        canvasGuidePanel.SetActive(false);
    }

    // --- Универсальный метод для всех кнопок "Назад" ---
    public void BackToMainMenu()
    {
        OpenMainMenu();
    }

    // --- Музыка ---
    void TurnMusicOff()
    {
        if (musicPlayer != null)
            musicPlayer.Pause();

        buttonMusicOn.SetActive(false);
        buttonMusicOff.SetActive(true);
    }

    void TurnMusicOn()
    {
        if (musicPlayer != null)
            musicPlayer.Play();

        buttonMusicOn.SetActive(true);
        buttonMusicOff.SetActive(false);
    }
}