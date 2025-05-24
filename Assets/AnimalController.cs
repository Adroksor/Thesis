using System.Collections;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AnimalController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.5f;
    public Vector2 walkTimeRange = new (1.5f, 3f);
    public Vector2 idleTimeRange = new (1.5f, 3f);
    
    [Header("Flee Settings")]
    public float fleeSpeed = 2f; 
    public float fleeDuration = 3f; 
    
    Rigidbody2D    rb;
    WalkTween   tweenWalk;
    SpriteRenderer sr;
    
    private Coroutine wanderRoutine;
    private Coroutine fleeRoutine;

    void OnEnable()
    {
        tweenWalk = GetComponentInChildren<WalkTween>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        GetComponent<EntityStatus>().onDamage += OnDamaged;
        wanderRoutine = StartCoroutine(WanderLoop());
    }

    void OnDisable()
    {
        GetComponent<EntityStatus>().onDamage -= OnDamaged;
        if (wanderRoutine != null) StopCoroutine(wanderRoutine);
        if (fleeRoutine   != null) StopCoroutine(fleeRoutine);
        rb.velocity = Vector2.zero;
    }

    void OnDamaged()
    {
        if (wanderRoutine != null)
            StopCoroutine(wanderRoutine);
        if (fleeRoutine != null)
            StopCoroutine(fleeRoutine);

        fleeRoutine = StartCoroutine(Flee());
    }

    IEnumerator Flee()
    {
        Vector2 dir = ((Vector2)transform.position - GameManager.instance.playerPosition).normalized;
        rb.velocity = dir * fleeSpeed;
        sr.flipX = dir.x < 0;

        yield return new WaitForSeconds(fleeDuration);

        rb.velocity = Vector2.zero;
        wanderRoutine = StartCoroutine(WanderLoop());
    }

    IEnumerator WanderLoop()
    {
        while (true)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            float   walkDuration = Random.Range(walkTimeRange.x, walkTimeRange.y);

            if (sr) sr.flipX = dir.x < 0;
            tweenWalk.PlayWalkTween();

            rb.velocity = dir * moveSpeed;
            yield return new WaitForSeconds(walkDuration);

            rb.velocity = Vector2.zero;
            tweenWalk.StopWalkTween();


            float idleDuration = Random.Range(idleTimeRange.x, idleTimeRange.y);
            yield return new WaitForSeconds(idleDuration);
        }
    }
}