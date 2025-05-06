using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PickupFloatingText : MonoBehaviour
{
    [SerializeField] private float floatDistance = 1.2f;
    [SerializeField] private float duration      = 1.0f;

    TextMeshProUGUI tmp;
    Color baseColor;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        baseColor = tmp.color;
    }

    public void Init(string message)
    {
        tmp.text  = message;
        tmp.color = baseColor;

        Vector3 targetPos = transform.localPosition + Vector3.up * floatDistance;

        // move up + fade out
        Sequence s = DOTween.Sequence()
            .Append(transform.DOLocalMove(targetPos, duration).SetEase(Ease.OutQuad))
            .Join(tmp.DOFade(0f, duration))
            .OnComplete(() => Destroy(gameObject));
    }
}