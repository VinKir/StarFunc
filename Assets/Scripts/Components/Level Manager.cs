#nullable enable

using System.Collections;
using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(LevelGenerator), typeof(ControlManagerDataHolder))]
public class LevelManager : MonoBehaviour
{
    public enum ControlManagers
    {
        ArgumentSlider,
        FunctionSelector,
    }

    // Пока что поддерживаем только заранее установленные уровни
    [Header("Level Generation"), SerializeField]
    private LevelDefinition? levelToLoad = null;

    [Header("References"), SerializeField]
    private Transform? circleObject = null;
    [SerializeField]
    private Transform? cameraObject = null;
    [SerializeField]
    private FunctionGraphGenerator? graphGenerator = null;
    [SerializeField]
    private GameUIManager? gameUIManager = null;

    private LevelGenerator? levelGenerator = null;
    private int starsCollected = 0;
    private int totalStars = 0;
    private IControlManager? controlManager = null;
    private ControlManagerDataHolder? controlManagerDataHolder = null;
    private Coroutine? levelTimerCoroutine = null;

    [Header("Background"), SerializeField]
    private SpriteRenderer? backgroundImage = null;

    public void CollectStar()
    {
        // Ограничиваем количество собранных звёзд числом всего количества звёзд на уровне
        starsCollected = starsCollected < totalStars ? starsCollected + 1 : totalStars;
        if (starsCollected == totalStars)
        {
            FinishLevel();
        }
    }

    public void Reset()
    {
        starsCollected = 0;

        if (levelTimerCoroutine != null)
        {
            StopCoroutine(levelTimerCoroutine);
            levelTimerCoroutine = null;
        }

        if (levelGenerator == null)
            return;

        levelGenerator.ClearLevel();

        levelToLoad = LevelProgressManager.Instance.SelectedLevel;

        if (levelToLoad == null)
            return;

        levelGenerator.ManuallyGenerateLevel(
            levelToLoad.levelFunction,
            levelToLoad.starPositions
        );

        if (backgroundImage != null)
        {
            if (levelToLoad.backgroundSprite != null)
            {
                backgroundImage.sprite = levelToLoad.backgroundSprite;
                backgroundImage.enabled = true;
            }
            else
            {
                backgroundImage.enabled = false;
            }
        }

        totalStars = levelToLoad.starPositions.Length;

        if (circleObject != null)
        {
            // Disable trail first
            if (circleObject.TryGetComponent<TrailRenderer>(out var trail))
            {
                trail.enabled = false;
                trail.Clear(); // Clears all points from the trail
            }

            // Move the circle to start position
            circleObject.position = new Vector3(
                levelToLoad.circlePosition.x,
                levelToLoad.circlePosition.y,
                circleObject.position.z
            );

            // Reset physics
            if (circleObject.TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.simulated = false;
            }

            // Clear trail again after moving to ensure no lingering positions
            if (trail != null)
                trail.Clear();
        }

        if (cameraObject != null)
        {
            cameraObject.position = new Vector3(
                levelToLoad.circlePosition.x + levelToLoad.cameraOffset.x,
                levelToLoad.circlePosition.y + levelToLoad.cameraOffset.y - cameraObject.GetComponent<Camera>().orthographicSize * 0.75f,
                cameraObject.position.z
            );
        }

        if (controlManager == null && graphGenerator != null)
        {
            graphGenerator.FunctionExpression = "0";
        }

        controlManager?.Reset();

        if (gameUIManager != null)
        {
            gameUIManager.PlaySprite();
        }
    }

    public void Play()
    {
        if (circleObject != null &&
            circleObject.TryGetComponent<Rigidbody2D>(out var rb) &&
            circleObject.TryGetComponent<TrailRenderer>(out var trail))
        {
            rb.simulated = true;
            trail.enabled = true;
        }

        if (levelToLoad != null && levelToLoad.maxRunningSeconds > 0f)
        {
            if (levelTimerCoroutine != null)
            {
                StopCoroutine(levelTimerCoroutine);
            }

            levelTimerCoroutine = StartCoroutine(LevelTimer(levelToLoad.maxRunningSeconds));
        }

        if (gameUIManager != null)
        {
            gameUIManager.StopSprite();
        }
    }

    public void FinishLevel()
    {
        Debug.Log($"Collected {starsCollected} out of {totalStars} stars.");

        var levelId = levelToLoad != null ? levelToLoad.levelIndex : 1;
        if (starsCollected > LevelProgressManager.Instance.GetStars(levelId))
            LevelProgressManager.Instance.SetStars(levelId, starsCollected);

        Reset();
    }

    private void Awake()
    {
        if (levelGenerator == null)
        {
            levelGenerator = GetComponent<LevelGenerator>();
        }

        if (controlManagerDataHolder == null)
        {
            controlManagerDataHolder = GetComponent<ControlManagerDataHolder>();
        }

        if (levelToLoad == null)
        {
            return;
        }

        controlManager = levelToLoad.controlManager switch
        {
            ControlManagers.ArgumentSlider => new ArgumentSliderControlManager(levelToLoad, controlManagerDataHolder),
            ControlManagers.FunctionSelector => new FunctionSelectorControlManager(levelToLoad, controlManagerDataHolder),
            _ => null,
        };

        Reset();
    }

    private IEnumerator LevelTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        FinishLevel();
    }
}