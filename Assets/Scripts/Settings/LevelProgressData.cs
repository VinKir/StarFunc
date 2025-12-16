#nullable enable
using System;

[Serializable]
public class LevelProgressData
{
    public int[] starsPerLevel;

    public LevelProgressData(int levelCount)
    {
        starsPerLevel = new int[levelCount];
    }

    public int GetStars(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= starsPerLevel.Length)
            return 0;

        return starsPerLevel[levelIndex];
    }

    public void SetStars(int levelIndex, int stars)
    {
        if (levelIndex < 0 || levelIndex >= starsPerLevel.Length)
            return;

        starsPerLevel[levelIndex] = Math.Clamp(stars, 0, 3);
    }
}
