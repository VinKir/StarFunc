#nullable enable

using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Public Fields

    [Header("Panels")]
    public GameObject? mainMenuPanel;

    [Header("Music Control")]
    public GameObject? buttonMusic;
    public Sprite? musicOnIcon;
    public Sprite? musicOffIcon;
    public AudioSource? musicPlayer;

    [Header("Buttons (Main Menu)")]
    public Button? playButton;
    public Button? settingsButton;
    public Button? guideButton;
    public Button? shopButton;

    [Header("Buttons (Settings Panel)")]
    public Button? aboutButtonButton;

    #endregion

    #region Private Fields

    private GameObject? currentPanel = null;

    #endregion

    #region Public Methods

    public void ToggleMusic()
    {
        if (SettingsManager.Instance.CurrentSettings.isMusicOn)
        {
            TurnMusicOff();
        }
        else
        {
            TurnMusicOn();
        }
    }

    public void TurnMusicOn()
    {
        if (musicPlayer == null || buttonMusic == null || musicOnIcon == null)
        {
            return;
        }

        musicPlayer.volume = 1f;
        buttonMusic.GetComponent<Button>().image.sprite = musicOnIcon;
        SettingsManager.Instance.SetMusicEnabled(true);
    }

    public void TurnMusicOff()
    {
        if (musicPlayer == null || buttonMusic == null || musicOffIcon == null)
        {
            return;
        }

        musicPlayer.volume = 0f;
        buttonMusic.GetComponent<Button>().image.sprite = musicOffIcon;
        SettingsManager.Instance.SetMusicEnabled(false);
    }

    public void ShowPanel(GameObject panel)
    {
        if (currentPanel == null || currentPanel == panel)
        {
            return;
        }

        currentPanel.SetActive(false);
        currentPanel = panel;
        currentPanel.SetActive(true);
    }

    #endregion

    #region Private Methods

    private void Awake()
    {
        if (SettingsManager.Instance.CurrentSettings.isMusicOn)
        {
            TurnMusicOn();
        }
        else
        {
            TurnMusicOff();
        }
    }

    private void Start()
    {
        currentPanel = mainMenuPanel;

        if (currentPanel == null)
        {
            Debug.LogError("UIManager: Main Menu Panel is not assigned.");
            return;
        }

        // --- ������ �������� ���� ---
        // playButton.onClick.AddListener(ShowPanel(gameLevelsPanel));
        // settingsButton.onClick.AddListener(ShowPanel(settingsPanel));
        // guideButton.onClick.AddListener(ShowPanel(guidePanel));
        // shopButton.onClick.AddListener(ShowPanel(shopPanel));

        // --- ������ �� �������� ---
        // aboutButtonButton.onClick.AddListener(OpenAboutPlay);
    }

    #endregion
}