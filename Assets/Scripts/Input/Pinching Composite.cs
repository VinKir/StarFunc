#if UNITY_EDITOR
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

// Инициализируется при загрузке редактора
[InitializeOnLoad]
#endif
/// <summary>
/// Составной binding для обнаружения жеста "щипок" (pinch) двумя пальцами.
/// Возвращает значение изменения расстояния между двумя точками касания.
/// Положительное значение = увеличение расстояния (zoom out), отрицательное = уменьшение (zoom in).
/// </summary>
[DisplayStringFormat("{firstTouch}+{secondTouch}")]
public class PinchingComposite : InputBindingComposite<float>
{
    /// <summary>
    /// Первая точка касания для жеста щипка.
    /// </summary>
    [InputControl(layout = "Value")]
    public int firstTouch;

    /// <summary>
    /// Вторая точка касания для жеста щипка.
    /// </summary>
    [InputControl(layout = "Value")]
    public int secondTouch;

    /// <summary>
    /// Масштаб для отрицательных значений (сближение пальцев - zoom in).
    /// </summary>
    public float negativeScale = 1f;

    /// <summary>
    /// Масштаб для положительных значений (удаление пальцев - zoom out).
    /// </summary>
    public float positiveScale = 1f;

    /// <summary>
    /// Компаратор для сравнения состояний касаний.
    /// Всегда возвращает 1, так как порядок не важен.
    /// </summary>
    private struct TouchStateComparer : IComparer<TouchState>
    {
        public readonly int Compare(TouchState x, TouchState y) => 1;
    }

    /// <summary>
    /// Вычисляет результирующее значение жеста щипка на основе двух точек касания.
    /// </summary>
    /// <param name="context">Контекст составного binding.</param>
    /// <returns>
    /// Значение изменения расстояния между касаниями:
    /// - Отрицательное значение = сближение пальцев (zoom in)
    /// - Положительное значение = расхождение пальцев (zoom out)
    /// - 0 = нет движения или касания не в фазе Moved
    /// </returns>
    public override float ReadValue(ref InputBindingCompositeContext context)
    {
        // Читаем состояние первого касания
        var touch_0 = context.ReadValue<TouchState, TouchStateComparer>(firstTouch);
        // Читаем состояние второго касания
        var touch_1 = context.ReadValue<TouchState, TouchStateComparer>(secondTouch);

        // Если оба касания не находятся в фазе движения, возвращаем 0
        if (touch_0.phase != TouchPhase.Moved || touch_1.phase != TouchPhase.Moved)
            return 0f;

        // Вычисляем начальное расстояние между точками касания
        var startDistance = math.distance(touch_0.startPosition, touch_1.startPosition);
        // Вычисляем текущее расстояние между точками касания
        var distance = math.distance(touch_0.position, touch_1.position);

        // Вычисляем изменение расстояния (делим startDistance на distance для инвертирования значения)
        var unscaledValue = startDistance / distance - 1f;
        // Применяем масштаб в зависимости от направления изменения
        return unscaledValue * (unscaledValue < 0 ? negativeScale : positiveScale);
    }

    /// <summary>
    /// Вычисляет текущую силу активации всего binding.
    /// </summary>
    /// <param name="context">Контекст составного binding.</param>
    /// <returns>Всегда возвращает 1 (полная активация).</returns>
    public override float EvaluateMagnitude(ref InputBindingCompositeContext context) => 1f;

    // Регистрирует составной binding в Input System при загрузке
    static PinchingComposite() => InputSystem.RegisterBindingComposite<PinchingComposite>();

    /// <summary>
    /// Инициализация перед загрузкой сцены для триггера статического конструктора.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init() { } // Запускаем статический конструктор
}