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
}
