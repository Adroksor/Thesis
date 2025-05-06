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
        Ease  swingEase       = Ease.OutQuad)
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
            .SetEase(swingEase)
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
        t.DOLocalMoveY(0f, 0.1f).SetRelative(false);
    }
    
    
    private const float squashFactor = 0.85f;   // 1 = no squash, 0.85 = 15 % smaller
    private const float squashTime   = 0.06f;   // seconds to reach the squash
    private const float reboundTime  = 0.12f;   // seconds to pop back
    private const Ease  bounceEase         = Ease.OutQuad;
    public static void PlayBounce(Transform t)
    {
        DOTween.Kill(t);

        Sequence seq = DOTween.Sequence().SetTarget(t);
        seq.Append(t.DOScale(t.localScale * squashFactor, squashTime).SetEase(bounceEase));
        seq.Append(t.DOScale(t.localScale, reboundTime).SetEase(Ease.OutBack)).SetLink(t.gameObject);
    }
    
}
