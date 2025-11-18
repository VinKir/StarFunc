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
    [SerializeField]
    private CameraMovementController? cameraMovementController = null;
    [SerializeField]
    private Image? lockUnlockButtonImage = null;
    [SerializeField]
    private Sprite? lockSprite = null;
    [SerializeField]
    private Sprite? unlockSprite = null;

    private bool isPlaying = false;
    private LevelManager? levelManager = null;

    public void ToggleLockButton()
    {
        if (cameraMovementController == null)
        {
            return;
        }

        var locked = cameraMovementController.lockOnCircle;
        cameraMovementController.lockOnCircle = !locked;

        if (lockUnlockButtonImage == null || lockSprite == null || unlockSprite == null)
        {
            return;
        }

        lockUnlockButtonImage.sprite = locked ? lockSprite : unlockSprite;
    }

    public void TogglePlayStopButton()
    {
        if (isPlaying && levelManager != null)
        {
            levelManager.Reset();
        }
        else if (levelManager != null)
        {
            levelManager.Play();
        }
    }

    public void PlaySprite()
    {
        if (playStopButtonImage == null || playSprite == null)
        {
            Debug.LogWarning("GameUIManager: Play/Stop button or sprites are not assigned.");
            return;
        }

        playStopButtonImage.sprite = playSprite;

        isPlaying = false;
    }

    public void StopSprite()
    {
        if (playStopButtonImage == null || stopSprite == null)
        {
            Debug.LogWarning("GameUIManager: Play/Stop button or sprites are not assigned.");
            return;
        }

        playStopButtonImage.sprite = stopSprite;

        isPlaying = true;
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