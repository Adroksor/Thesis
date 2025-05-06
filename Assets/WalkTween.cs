using System;
using UnityEngine;
using DG.Tweening;

public class WalkTween : MonoBehaviour
{
    [Header("Tween Settings")]
    [Tooltip("How far up/right the pig stretches")]
    public Vector2 stretchOffset = new Vector2(0.08f, 0.08f);

    [Tooltip("How much to squash at the bottom point ( < 1 = thinner )")]
    [Range(0.7f, 1f)] public float squashScale = 0.85f;

    [Tooltip("Seconds for each leg of the V movement")]
    public float legTime = 0.15f;

    
    Vector3 basePos;      // the real, never‑changing origin
    Vector3 baseScale;
    void Awake()
    {
        if (transform.localScale == Vector3.zero)
            transform.localScale = Vector3.one;
        baseScale = transform.localScale;
        basePos   = transform.localPosition;
        
    }
    
    public void PlayWalkTween()
    {
        DOTween.Kill(transform, "WalkTween");

        transform.localPosition = basePos;
        transform.localScale    = baseScale;

        Vector3 stretchRight = basePos + (Vector3)stretchOffset;
        Vector3 stretchLeft  = basePos + new Vector3(-stretchOffset.x, stretchOffset.y, 0);
        Vector3 squashVec = new Vector3(baseScale.x * squashScale,
            baseScale.y / squashScale,
            baseScale.z);

        Sequence seq = DOTween.Sequence().SetId("WalkTween").SetTarget(transform);

        seq.Append(transform.DOLocalMove(stretchRight, legTime).SetEase(Ease.OutSine));
        seq.Join  (transform.DOScale(squashVec,        legTime).SetEase(Ease.OutSine));

        seq.Append(transform.DOLocalMove(basePos, legTime).SetEase(Ease.InSine));
        seq.Join  (transform.DOScale(baseScale,  legTime).SetEase(Ease.InSine));

        seq.Append(transform.DOLocalMove(stretchLeft,  legTime).SetEase(Ease.OutSine));
        seq.Join  (transform.DOScale(squashVec,        legTime).SetEase(Ease.OutSine));

        seq.Append(transform.DOLocalMove(basePos, legTime).SetEase(Ease.InSine));
        seq.Join  (transform.DOScale(baseScale,  legTime).SetEase(Ease.InSine));

        seq.SetLoops(-1).SetLink(gameObject);
    }


    public void StopWalkTween()
    {
        // Rewind to base pose, then kill
        DOTween.Kill(transform, "WalkTween", true);

        // Make sure pose is exactly the baseline
        transform.localPosition = basePos;
        transform.localScale    = baseScale;
    }
}