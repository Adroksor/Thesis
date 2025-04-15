using UnityEngine;
using UnityEngine.UI;
using System;

public class CircularProgressBar : MonoBehaviour
{
    public Image progressImage; // Assign in Inspector
    public float timeToFinish = 5f; // Total time to fill the circle
    private float timer = 0f;
    private bool isRunning = false;
    private Action currentCompletionCallback; // Use a different variable to avoid confusion with persistent listeners if needed

    // You might not need this public 'onTimeout' if callbacks are always
    // provided via StartProgress. Decide based on your design.
    // If you keep it, be mindful of += vs = assignment.
    public Action onTimeout;

    // Removed Start() as it was empty

    public void StartProgress(float duration, Action callback = null)
    {
        timeToFinish = duration;
        timer = 0f;
        progressImage.fillAmount = 0f; // Explicitly reset fill amount
        currentCompletionCallback = callback; // Store the callback for this specific progress run
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