#nullable enable

using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(LevelGenerator), typeof(ControlManagerDataHolder))]
public class LevelManager : MonoBehaviour
{
    public enum ControlManagers
    {
        ArgumentSlider
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

    private LevelGenerator? levelGenerator = null;
    private int starsCollected = 0;
    private int totalStars = 0;
    private IControlManager? controlManager = null;
    private ControlManagerDataHolder? controlManagerDataHolder = null;

    public void CollectStar()
    {
        // Ограничиваем количество собранных звёзд числом всего количества звёзд на уровне
        starsCollected = starsCollected < totalStars ? starsCollected + 1 : totalStars;
    }

    public void Reset()
    {
        starsCollected = 0;

        if (levelGenerator == null)
        {
            return;
        }

        levelGenerator.ClearLevel();

        if (levelToLoad == null)
        {
            return;
        }

        levelGenerator.ManuallyGenerateLevel(
            levelToLoad.levelFunction,
            levelToLoad.starPositions
        );

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
            {
                trail.Clear();
            }
        }

        if (cameraObject != null)
        {
            cameraObject.position = new Vector3(
                levelToLoad.circlePosition.x,
                levelToLoad.circlePosition.y - cameraObject.GetComponent<Camera>().orthographicSize * 0.75f,
                cameraObject.position.z
            );
        }

        if (graphGenerator != null)
        {
            graphGenerator.FunctionExpression = "0";
            graphGenerator.ComputeFunctionGraph(true);
        }

        controlManager?.Reset();
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
            _ => null,
        };

        Reset();
    }
}