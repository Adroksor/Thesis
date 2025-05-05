using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public static class TweenHelper
{
    
    public static void SwingOnce(
        Transform t,
        float angle      = 45f,
        float forwardDur = 0.1f,
        float returnDur  = 0.13f,
        Ease  ease       = Ease.OutQuad)
    {
        DOTween.Kill(t);

        Sequence seq = DOTween.Sequence()
            .Append(t.DOLocalRotate(
                new Vector3(0, 0, -angle),
                forwardDur,
                RotateMode.LocalAxisAdd))
            .Append(t.DOLocalRotate(
                Vector3.zero,
                returnDur))
            .SetEase(ease)
            .SetTarget(t);
    }
    
    private const float EatDistance  = 0.08f;   // meters the item moves up / down
    private const float EatDuration  = 0.12f;   // seconds for each half‑cycle
    private const Ease  EatEase      = Ease.InOutSine;
    public static void StartEatBounce(Transform t)
    {
        DOTween.Kill(t, "EatBounce");

        Sequence seq = DOTween.Sequence()
            .SetId("EatBounce")
            .SetTarget(t)
            .Append(t.DOLocalMoveY(EatDistance, EatDuration)
                .SetRelative()
                .SetEase(EatEase))
            .Append(t.DOLocalMoveY(-EatDistance, EatDuration)
                .SetRelative()
                .SetEase(EatEase))
            .SetLoops(-1)
            .SetLink(t.gameObject);
    }

    public static void StopEatBounce(Transform t)
    {
        DOTween.Kill(t, "EatBounce");
        // ensure we return exactly to original localPosition
        // (comment this line if you prefer to leave it wherever it stopped)
        t.DOLocalMoveY(0f, 0.05f).SetRelative(false);
    }
    
}
