using UnityEngine;
using UnityEngine.UI;
using System;

public class CircularProgressBar : MonoBehaviour
{
    public Image progressImage;
    public float timeToFinish = 5f;
    private float timer = 0f;
    private bool isRunning = false;
    private Action currentCompletionCallback;
    public Action onTimeout;

    // Removed Start() as it was empty

    public void StartProgress(float duration, Action callback = null)
    {
        timeToFinish = duration;
        timer = 0f;
        progressImage.fillAmount = 0f;
        currentCompletionCallback = callback;
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

            // Invoke the callback specifically provided for this run
            currentCompletionCallback?.Invoke();
            currentCompletionCallback = null; // Clear it after invoking

            // If you still want the public onTimeout for other persistent events,
            // you can invoke it here as well, but it seems redundant
            // with the callback parameter logic.
            // onTimeout?.Invoke();
        }
    }

    // Optional: Method to stop the progress prematurely if needed
    public void StopProgress()
    {
        isRunning = false;
        timer = 0f;
        progressImage.fillAmount = 0f;
        currentCompletionCallback = null; // Ensure callback isn't held
    }
}