using UnityEngine;
using DG.Tweening;
using TMPro;

public class TheoryAnimationController : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lineRenderer;
    public TextMeshProUGUI functionText;
    public FunctionGraphGenerator graphGenerator;

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
        StopAnimation();

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
            case AnimationType.LinearFunction4:
                AnimateLinearFunction4();
                break;
            case AnimationType.LinearFunction5:
                AnimateLinearFunction5();
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
        Clear();

        k = 0;
        b = 0;
        AnimateLine();

        Sequence seq = DOTween.Sequence();

        seq.Append(
            DOTween.To(() => k, v => { k = v; AnimateLine(); }, 1f, 2f)
            .SetEase(Ease.InOutSine)
        );
        seq.Append(
            DOTween.To(() => k, v => { k = v; AnimateLine(); }, 0f, 2f)
            .SetEase(Ease.InOutSine)
        );

        seq.Append(
            DOTween.To(() => b, v => { b = v; AnimateLine(); }, 1f, 1.5f)
            .SetEase(Ease.InOutSine)
        );
        seq.Append(
            DOTween.To(() => b, v => { b = v; AnimateLine(); }, 0f, 1.5f)
            .SetEase(Ease.InOutSine)
        );

        seq.Append(
            DOTween.To(() => k, v => { k = v; AnimateLine(); }, -1f, 2f)
        );
        seq.Join(
            DOTween.To(() => b, v => { b = v; AnimateLine(); }, 2f, 2f)
        );

        seq.Append(
            DOTween.To(() => k, v => { k = v; AnimateLine(); }, 0f, 1.5f)
        );
        seq.Join(
            DOTween.To(() => b, v => { b = v; AnimateLine(); }, 0f, 1.5f)
        );

        seq.SetLoops(-1, LoopType.Restart);

        currentTween = seq;
    }

    void AnimateLinearFunction2()
    {
        Clear();

        k = 0;
        b = 0;
        AnimateLine();

        currentTween = DOTween.Sequence()
            .Append(DOTween.To(() => k, v => { k = v; AnimateLine(); }, 5f, 3f)
                .SetEase(Ease.OutSine))
            .AppendInterval(0.5f)
            .Append(DOTween.To(() => k, v => { k = v; AnimateLine(); }, 1f, 3f)
                .SetEase(Ease.InOutSine))
            .AppendInterval(0.3f)
            .Append(DOTween.To(() => k, v => { k = v; AnimateLine(); }, 0f, 2f)
                .SetEase(Ease.InOutSine))
            .SetLoops(-1, LoopType.Restart);
    }

    void AnimateLinearFunction3()
    {
        Clear();

        k = 0;
        b = 0;
        AnimateLine();

        currentTween = DOTween.Sequence()
            .Append(DOTween.To(() => k, v => { k = v; AnimateLine(); }, -5f, 3f)
                .SetEase(Ease.OutSine))
            .AppendInterval(0.5f)
            .Append(DOTween.To(() => k, v => { k = v; AnimateLine(); }, -1f, 3f)
                .SetEase(Ease.InOutSine))
            .AppendInterval(0.3f)
            .Append(DOTween.To(() => k, v => { k = v; AnimateLine(); }, 0f, 2f)
                .SetEase(Ease.InOutSine))
            .SetLoops(-1, LoopType.Restart);
    }

    void AnimateLinearFunction4()
    {
        Clear();

        k = 0;
        b = 0;
        AnimateLine();

        currentTween = DOTween.Sequence()
            .Append(DOTween.To(() => b, v => { b = v; AnimateLine(); }, 2f, 2.5f)
                .SetEase(Ease.InOutSine))
            .Append(DOTween.To(() => b, v => { b = v; AnimateLine(); }, -2f, 2.5f)
                .SetEase(Ease.InOutSine))
            .Append(DOTween.To(() => b, v => { b = v; AnimateLine(); }, 0f, 2f)
                .SetEase(Ease.InOutSine))
            .SetLoops(-1, LoopType.Restart);
    }

    void AnimateLinearFunction5()
    {
        Clear();

        k = 1f;
        b = 0;
        AnimateLine();

        currentTween = DOTween.Sequence()
            .Append(DOTween.To(() => b, v => { b = v; AnimateLine(); }, 2f, 2.5f)
                .SetEase(Ease.OutQuad))
            .AppendInterval(0.3f)
            .Append(DOTween.To(() => b, v => { b = v; AnimateLine(); }, -1f, 2f)
                .SetEase(Ease.OutQuad))
            .AppendInterval(0.3f)
            .Append(DOTween.To(() => b, v => { b = v; AnimateLine(); }, 0f, 2f)
                .SetEase(Ease.InOutSine))
            .SetLoops(-1, LoopType.Restart);
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

        graphGenerator.FunctionExpression = $"{k:F2}x + {b:F2}";
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


    public void StopAnimation()
    {
        currentTween?.Kill();
        Clear();
    }
}

