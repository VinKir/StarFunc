using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject canvasMainMenuPanel;
    public GameObject canvasSettingsPanel;
    public GameObject canvasGuidePanel;
    public GameObject canvasAboutPlayPanel;
    public GameObject canvasGameLevelsPanel;
    public GameObject canvasShopPanel;

    [Header("Music Control")]
    public GameObject buttonMusicOn;
    public GameObject buttonMusicOff;
    public AudioSource musicPlayer;

    [Header("Buttons (Main Menu)")]
    public Button playButton;
    public Button settingsButton;
    public Button guideButton;
    public Button shopButton;

    [Header("Buttons (Settings Panel)")]
    public Button aboutButtonButton; // ������� � CanvasAboutPlayPanel

    void Start()
    {
        // --- ������ �������� ���� ---
        playButton.onClick.AddListener(OpenGameLevels);
        settingsButton.onClick.AddListener(OpenSettings);
        guideButton.onClick.AddListener(OpenGuide);
        shopButton.onClick.AddListener(OpenShop);

        // --- ������ �� �������� ---
        aboutButtonButton.onClick.AddListener(OpenAboutPlay);

        // --- ������ ---
        buttonMusicOn.GetComponent<Button>().onClick.AddListener(TurnMusicOff);
        buttonMusicOff.GetComponent<Button>().onClick.AddListener(TurnMusicOn);

        // --- ��������� ������ ---
        OpenMainMenu();
    }

    // --- ������ ��������� ---
    public void OpenSettings()
    {
        HideAllPanels();
        canvasSettingsPanel.SetActive(true);
    }

    public void OpenGuide()
    {
        HideAllPanels();
        canvasGuidePanel.SetActive(true);
    }

    public void OpenGameLevels()
    {
        HideAllPanels();
        canvasGameLevelsPanel.SetActive(true);
    }

    public void OpenAboutPlay()
    {
        HideAllPanels();
        canvasAboutPlayPanel.SetActive(true);
    }

    public void OpenShop()
    {
        HideAllPanels();
        canvasShopPanel.SetActive(true);
    }

    public void OpenMainMenu()
    {
        HideAllPanels();
        canvasMainMenuPanel.SetActive(true);
    }

    // --- ������������� ������� ---
    public void BackToMainMenu()
    {
        OpenMainMenu();
    }

    // --- ������ ---
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

    // --- �������� ��� ������ ---
    void HideAllPanels()
    {
        canvasMainMenuPanel.SetActive(false);
        canvasSettingsPanel.SetActive(false);
        canvasGuidePanel.SetActive(false);
        canvasAboutPlayPanel.SetActive(false);
        canvasGameLevelsPanel.SetActive(false);
        canvasShopPanel.SetActive(false);
    }
}