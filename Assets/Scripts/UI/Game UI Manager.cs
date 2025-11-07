#nullable enable

using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [Header("Play/Stop Button"), SerializeField]
    private Image? playStopButtonImage = null;
    [SerializeField]
    private Sprite? playSprite = null;
    [SerializeField]
    private Sprite? stopSprite = null;

    private bool isPlaying = false;
    private LevelManager? levelManager = null;

    public void TogglePlayStopButton()
    {
        if (playStopButtonImage == null || playSprite == null || stopSprite == null)
        {
            Debug.LogWarning("GameUIManager: Play/Stop button or sprites are not assigned.");
            return;
        }

        playStopButtonImage.sprite = isPlaying ? playSprite : stopSprite;

        if (isPlaying && levelManager != null)
        {
            levelManager.Reset();
        }
        else if (levelManager != null)
        {
            levelManager.Play();
        }

        isPlaying = !isPlaying;
    }

    private void Awake()
    {
        levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("GameUIManager: LevelManager not found in the scene.");
        }
    }
}