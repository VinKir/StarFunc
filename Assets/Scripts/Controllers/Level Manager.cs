#nullable enable

using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(LevelGenerator))]
class LevelManager : MonoBehaviour
{
    // Пока что поддерживаем только заранее установленные уровни
    [Header("Level Generation"), SerializeField]
    private LevelDefinition? levelToLoad = null;

    private LevelGenerator? levelGenerator = null;

    private void Awake()
    {
        if (levelGenerator == null)
        {
            levelGenerator = GetComponent<LevelGenerator>();
        }

        if (levelToLoad != null)
        {
            levelGenerator.ManuallyGenerateLevel(
                levelToLoad.levelFunction,
                levelToLoad.starPositions,
                levelToLoad.circlePosition);
        }
    }
}