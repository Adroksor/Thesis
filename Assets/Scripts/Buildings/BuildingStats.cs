using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BuildingStats : MonoBehaviour
{
    public int health;
    public int maxHealth;

    public Action destroyMe;
    
    public void TakeDamage(int damage)
    {
        PlayBounce();
        health -= damage;
        if (health <= 0)
        {
            destroyMe?.Invoke();
        }
    }
    
    
    private float squashFactor = 0.85f;   // 1 = no squash, 0.85 = 15 % smaller
    private float squashTime   = 0.06f;   // seconds to reach the squash
    private float reboundTime  = 0.12f;   // seconds to pop back
    private Ease  ease         = Ease.OutQuad;

    Vector3 originalScale;

    void Awake() => originalScale = transform.localScale;

    public void PlayBounce()
    {
        DOTween.Kill(transform);

        Sequence seq = DOTween.Sequence().SetTarget(transform);
        seq.Append(transform.DOScale(originalScale * squashFactor, squashTime).SetEase(ease));
        seq.Append(transform.DOScale(originalScale, reboundTime).SetEase(Ease.OutBack)).SetLink(gameObject);
    }
}
