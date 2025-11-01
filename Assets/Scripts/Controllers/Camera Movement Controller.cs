#nullable enable

using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Контроллер движения 2D ортографической камеры.
/// Обрабатывает перемещение камеры и зум с помощью Input System.
/// </summary>
[RequireComponent(typeof(Camera))]
class CameraMovementController : MonoBehaviour
{
    [Header("Zoom Settings")]
    [Tooltip("Скорость изменения зума")]
    public float zoomSpeed = 2f;
    [Tooltip("Минимальный размер ортографической камеры")]
    public float minZoom = 3f;
    [Tooltip("Максимальный размер ортографической камеры")]
    public float maxZoom = 20f;

    [Header("Movement Settings")]
    public bool lockOnCircle = false;
    public float lockedOnCircleReactiveDistanceFromView = 0.1f;
    [Tooltip("Скорость следования камеры за объектом (выше = быстрее)")]
    public float followSmoothSpeed = 5f;

    [Header("References")]
    public Transform? circleObject = null;

    private Camera? cam = null;
    private InputSystemDefaultActions? inputActions = null;
    private Vector2 moveInput = Vector2.zero;

    /// <summary>
    /// Инициализация компонентов и подписка на события ввода.
    /// </summary>
    private void Awake()
    {
        cam = GetComponent<Camera>();

        // Создаём экземпляр Input Actions
        inputActions = new InputSystemDefaultActions();

        // Подписываемся на событие зума
        inputActions.Player.Zoom.performed += OnZoom;
    }

    /// <summary>
    /// Включение системы ввода.
    /// </summary>
    private void OnEnable()
    {
        inputActions?.Player.Enable();
    }

    /// <summary>
    /// Отключение системы ввода.
    /// </summary>
    private void OnDisable()
    {
        inputActions?.Player.Disable();
    }

    /// <summary>
    /// Очистка подписок при уничтожении объекта.
    /// </summary>
    private void OnDestroy()
    {
        if (inputActions != null)
        {
            inputActions.Player.Zoom.performed -= OnZoom;
            inputActions.Dispose();
        }
    }

    /// <summary>
    /// Обновление каждый кадр. Обрабатывает движение камеры.
    /// </summary>
    private void Update()
    {
        if (!lockOnCircle)
        {
            HandleMovement();
        }
    }

    /// <summary>
    /// Фиксированное обновление. Обрабатывает следование за кругом.
    /// Используется FixedUpdate, так как круг двигается в FixedUpdate.
    /// </summary>
    private void FixedUpdate()
    {
        if (lockOnCircle)
        {
            HandleCircleMovement();
        }
    }

    private void HandleCircleMovement()
    {
        if (circleObject == null || cam == null)
        {
            return;
        }

        var camHalfWidth = cam.orthographicSize * cam.aspect;
        var camHalfHeight = cam.orthographicSize;

        // Calculate the threshold boundaries
        float thresholdX = camHalfWidth * (1 - lockedOnCircleReactiveDistanceFromView);
        float thresholdY = camHalfHeight * (1 - lockedOnCircleReactiveDistanceFromView);

        // Calculate target position (where camera should be to keep circle centered)
        Vector3 targetPosition = circleObject.position;
        targetPosition.z = transform.position.z; // Preserve camera's Z position

        // Calculate delta from current position
        Vector3 delta = targetPosition - transform.position;

        // Only move if outside the threshold zone
        float absX = Mathf.Abs(delta.x);
        float absY = Mathf.Abs(delta.y);

        if (absX > thresholdX || absY > thresholdY)
        {
            // Calculate how much we're over the threshold
            float overX = Mathf.Max(0, absX - thresholdX);
            float overY = Mathf.Max(0, absY - thresholdY);

            // Create movement vector that only corrects the excess
            Vector3 moveVector = new Vector3(
                overX * Mathf.Sign(delta.x),
                overY * Mathf.Sign(delta.y),
                0f
            );

            // Smooth interpolation using Lerp for consistent, frame-rate independent movement
            Vector3 smoothMove = Vector3.Lerp(Vector3.zero, moveVector, followSmoothSpeed * Time.deltaTime);
            transform.position += smoothMove;
        }
    }

    /// <summary>
    /// Обрабатывает перемещение камеры на основе ввода Move.
    /// Камера перемещается на точное расстояние изменения позиции касания в пикселях.
    /// Для мыши требуется удержание левой кнопки, для touch работает всегда.
    /// </summary>
    private void HandleMovement()
    {
        if (inputActions == null || cam == null)
        {
            return;
        }

        // Читаем текущее значение ввода движения (delta position в пикселях)
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        // Проверяем, используется ли мышь или touch
        // Для мыши проверяем нажатие левой кнопки, для touch проверка не нужна
        bool isMouseButtonPressed = Mouse.current != null && Mouse.current.leftButton.isPressed;
        bool isTouchActive = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed;

        // Если есть ввод и выполнены условия (мышь нажата или touch активен), перемещаем камеру
        if (moveInput != Vector2.zero && (isMouseButtonPressed || isTouchActive))
        {
            // Конвертируем пиксельную дельту в мировые координаты
            // Для ортографической камеры: worldUnits = pixels * (orthographicSize * 2) / screenHeight
            float pixelsToWorldUnits = cam.orthographicSize * 2f / Screen.height;

            // Применяем конвертацию к дельте движения
            Vector3 worldDelta = new(
                moveInput.x * pixelsToWorldUnits,
                moveInput.y * pixelsToWorldUnits,
                0f
            );

            // Перемещаем камеру в противоположном направлении (инвертируем, чтобы контент двигался с пальцем)
            transform.position -= worldDelta;
        }
    }

    /// <summary>
    /// Обрабатывает зум камеры на основе ввода Zoom.
    /// </summary>
    /// <param name="context">Контекст callback от Input System.</param>
    private void OnZoom(InputAction.CallbackContext context)
    {
        if (cam == null)
        {
            return;
        }

        // Читаем значение зума (обычно от колёсика мыши)
        float zoomValue = context.ReadValue<float>();

        // Изменяем размер ортографической камеры
        float newSize = cam.orthographicSize - zoomValue * zoomSpeed * Time.deltaTime;

        // Ограничиваем зум в заданных пределах
        cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
    }
}