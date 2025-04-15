using UnityEngine;
using UnityEngine.UI;
using System;

public class CircularProgressBar : MonoBehaviour
{
    public Image progressImage; // Assign in Inspector
    public float timeToFinish = 5f; // Total time to fill the circle
    private float timer = 0f;
    private bool isRunning = false;
    public Action onTimeout; // Optional callback

    private void Start()
    {
        StartProgress(5);
    }

    public void StartProgress(float duration, Action callback = null)
    {
        timeToFinish = duration;
        timer = 0f;
        isRunning = true;
    }

    void Update()
    {
        if (!isRunning) return;

        timer += Time.deltaTime;
        float progress = Mathf.Clamp01(timer / timeToFinish);
        progressImage.fillAmount = progress;

        if (timer >= timeToFinish)
        {
            isRunning = false;
            onTimeout?.Invoke(); // Call the timeout function if set
            Debug.Log("finish");
        }
    }
}