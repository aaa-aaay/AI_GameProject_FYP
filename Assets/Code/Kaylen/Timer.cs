using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimerUI : MonoBehaviour
{
    [Header("Timer Settings")]
    public float maxTime = 30f;

    [Header("Exit UI")]
    public CanvasGroup exitCanvasGroup;
    public float fadeDuration = 1f;
    public float displayDuration = 3f;

    [Header("Arrow UI")]
    public RectTransform arrowUI;          // Rotating arrow
    public List<GameObject> timerUIObjects;

    private float currentTime;
    private bool isRunning = false;
    private bool hasTriggeredExit = false;

    void Start()
    {
        ResetTimer();
        if (exitCanvasGroup != null)
            exitCanvasGroup.alpha = 0f;

        SetTimerUIActive(false);
        UpdateArrowRotation();
    }

    void Update()
    {
        if (!isRunning)
            return;

        currentTime -= Time.deltaTime;
        if (currentTime < 0f) currentTime = 0f;

        UpdateArrowRotation();
        DebugCurrentTime();

        if (currentTime <= 0f && !hasTriggeredExit)
        {
            hasTriggeredExit = true;
            isRunning = false;
            ShowExitCanvas();
        }
    }

    void UpdateArrowRotation()
    {
        if (arrowUI == null || maxTime <= 0f)
            return;

        float progress = Mathf.Clamp01(currentTime / maxTime);

        float startAngle = 0f;
        float endAngle = 90f;

        float angle = Mathf.Lerp(startAngle, endAngle, progress);
        angle = Mathf.Clamp(angle, startAngle, endAngle);

        arrowUI.localEulerAngles = new Vector3(0, 0, angle);
    }

    void ShowExitCanvas()
    {
        if (exitCanvasGroup == null)
        {
            Debug.LogWarning("Exit CanvasGroup not assigned!");
            return;
        }

        StopAllCoroutines();
        StartCoroutine(FadeExitCanvas());
    }

    IEnumerator FadeExitCanvas()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            exitCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        exitCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(displayDuration);

        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            exitCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        exitCanvasGroup.alpha = 0f;
    }

    public void StartTimer()
    {
        ResetTimer();
        isRunning = true;
        SetTimerUIActive(true);
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        currentTime = maxTime;
        hasTriggeredExit = false;

        if (exitCanvasGroup != null)
            exitCanvasGroup.alpha = 0f;

        UpdateArrowRotation();
    }

    void SetTimerUIActive(bool state)
    {
        foreach (var obj in timerUIObjects)
        {
            if (obj != null)
                obj.SetActive(state);
        }
    }

    public void DebugCurrentTime()
    {
        //Debug.Log($"[TimerUI] Current Time: {currentTime:F2}");
    }

    public float GetRemainingTime() => currentTime;
}
