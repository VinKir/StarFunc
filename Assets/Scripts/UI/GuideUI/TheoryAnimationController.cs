using UnityEngine;
using DG.Tweening;
using TMPro;

public class TheoryAnimationController : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lineRenderer;
    public TextMeshProUGUI functionText;

    private float k, b;
    private Tween currentTween;
    private const float LENGTH = 10f;
    private const int POINTS = 100;

    void Awake()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();
    }

    public void PlayAnimation(AnimationType type)
    {
        currentTween?.Kill();
        Clear();

        switch (type)
        {
            case AnimationType.LinearFunction1:
                AnimateLinearFunction1();
                break;
            case AnimationType.LinearFunction2:
                AnimateLinearFunction2();
                break;
            case AnimationType.LinearFunction3:
                AnimateLinearFunction3();
                break;
            case AnimationType.TrigSin:
                AnimateSin();
                break;
            case AnimationType.TrigCos:
                AnimateCos();
                break;
            default:
                Clear();
                break;
        }
    }

    void AnimateLinearFunction1()
    {
        // y = kx + 0, k changes 0 → 1 → 0
        k = 0;
        b = 0;
        AnimateLine();
        currentTween = DOTween.To(() => k, value =>
        {
            k = value;
            AnimateLine();
        }, 1f, 4f)
        .SetLoops(-1, LoopType.Yoyo)
        .SetEase(Ease.InOutSine);
    }

    void AnimateLinearFunction2()
    {
        // y = kx + 0, k changes 1 → -1 → 1
        k = 1;
        b = 0;
        AnimateLine();
        currentTween = DOTween.To(() => k, value =>
        {
            k = value;
            AnimateLine();
        }, -1f, 6f)
        .SetLoops(-1, LoopType.Yoyo)
        .SetEase(Ease.InOutSine);
    }

    void AnimateLinearFunction3()
    {
        // y = x + b, b changes 0 → 3 → -3
        k = 1;
        b = -3;
        AnimateLine();
        currentTween = DOTween.To(() => b, value =>
        {
            b = value;
            AnimateLine();
        }, 3f, 4f)
        .SetLoops(-1, LoopType.Yoyo)
        .SetEase(Ease.InOutSine);
    }

    void AnimateLine()
    {
        if (lineRenderer == null) return;

        lineRenderer.positionCount = POINTS;

        float halfLen = LENGTH / 2f;

        Vector2 dir = new Vector2(1, k).normalized;

        Vector2 offset = dir * halfLen;

        Vector2 center = new Vector2(0, b);

        Vector2 start = center - offset;
        Vector2 end = center + offset;

        for (int i = 0; i < POINTS; i++)
        {
            float t = (float)i / (POINTS - 1);
            Vector2 pos = Vector2.Lerp(start, end, t);
            lineRenderer.SetPosition(i, new Vector3(pos.x, pos.y, 0));
        }

        if (functionText)
            functionText.text = $"y = {k:F2}x + {b:F2}";
    }

    void AnimateSin()
    {
        Clear();
        int points = 80;
        lineRenderer.positionCount = points;

        DOTween.To(() => 0f, phase =>
        {
            for (int i = 0; i < points; i++)
            {
                float x = (i - points / 2f) / 10f;
                float y = Mathf.Sin(x + phase);
                lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            }
        }, Mathf.PI * 2f, 4f).SetLoops(-1, LoopType.Incremental);
    }

    void AnimateCos()
    {
        Clear();
        int points = 80;
        lineRenderer.positionCount = points;

        DOTween.To(() => 0f, phase =>
        {
            for (int i = 0; i < points; i++)
            {
                float x = (i - points / 2f) / 10f;
                float y = Mathf.Cos(x + phase);
                lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            }
        }, Mathf.PI * 2f, 4f).SetLoops(-1, LoopType.Incremental);
    }

    void Clear()
    {
        lineRenderer.positionCount = 0;
        functionText.text = "";
    }
}

