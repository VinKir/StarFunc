#nullable enable

using System;

/// <summary>
/// Структура для хранения игровых настроек.
/// Сериализуется для сохранения настроек между сеансами игры.
/// </summary>
[Serializable]
public struct GameSettings
{
    /// <summary>
    /// Включена ли фоновая музыка.
    /// </summary>
    public bool isMusicOn;

    /// <summary>
    /// Включены ли звуковые эффекты.
    /// </summary>
    public bool isSoundEffectsOn;

    /// <summary>
    /// Сид для генерации уровней.
    /// </summary>
    public int seed;

    /// <summary>
    /// Возвращает настройки по умолчанию.
    /// </summary>
    /// <returns>Структура GameSettings с включёнными музыкой и звуковыми эффектами.</returns>
    public static GameSettings Default()
    {
        return new GameSettings
        {
            isMusicOn = true, // По умолчанию музыка включена
            isSoundEffectsOn = true, // По умолчанию звуковые эффекты включены
            seed = new Random().Next()
        };
    }
}