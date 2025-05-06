using System.Collections;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AnimalWander : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.5f;
    public Vector2 walkTimeRange = new Vector2(1.5f, 3f);
    public Vector2 idleTimeRange = new Vector2(1.5f, 3f);

    Rigidbody2D    rb;
    WalkTween   tweenWalk;
    SpriteRenderer sr;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        tweenWalk = GetComponentInChildren<WalkTween>();   // now on child
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    void OnEnable()  => StartCoroutine(WanderLoop());
    void OnDisable() { StopAllCoroutines(); rb.velocity = Vector2.zero; }

    IEnumerator WanderLoop()
    {
        while (true)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            float   walkDuration = Random.Range(walkTimeRange.x, walkTimeRange.y);

            if (sr) sr.flipX = dir.x < 0;     // face sprite
            tweenWalk.PlayWalkTween();

            rb.velocity = dir * moveSpeed;    // start moving
            yield return new WaitForSeconds(walkDuration);

            rb.velocity = Vector2.zero;
            tweenWalk.StopWalkTween();


            float idleDuration = Random.Range(idleTimeRange.x, idleTimeRange.y);
            yield return new WaitForSeconds(idleDuration);
        }
    }
}