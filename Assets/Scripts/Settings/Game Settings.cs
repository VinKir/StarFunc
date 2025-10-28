#nullable enable

using System;

[Serializable]
public struct GameSettings
{
    public bool isMusicOn;
    public bool isSoundEffectsOn;

    public static GameSettings Default()
    {
        return new GameSettings
        {
            isMusicOn = true,
            isSoundEffectsOn = true
        };
    }
}