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

    [Header("Exit Spawn Settings")]
    public GameObject exitPrefab;
    public List<Transform> spawnPoints;

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

        // Debug function
        DebugCurrentTime();

        // Trigger exit UI only once
        if (currentTime <= 0f && !hasTriggeredExit)
        {
            hasTriggeredExit = true;
            isRunning = false;

            ShowExitCanvas();
            SpawnExit();
        }
    }

    void UpdateArrowRotation()
    {
        if (arrowUI == null || maxTime <= 0f)
            return;

        // Normalize progress: 1 = full time, 0 = timer finished
        float progress = Mathf.Clamp01(currentTime / maxTime);

        // Top = 0°, Left = 90° (anti-clockwise)
        float startAngle = 0f;    // Top
        float endAngle = 90f;     // Left

        // Interpolate anti-clockwise
        float angle = Mathf.Lerp(startAngle, endAngle, 1f - progress);

        // Clamp to prevent overshooting
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

    void SpawnExit()
    {
        if (exitPrefab == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("Exit prefab or spawn points not assigned!");
            return;
        }

        Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        Instantiate(exitPrefab, randomPoint.position, randomPoint.rotation);
        Debug.Log("Exit spawned at: " + randomPoint.name);
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

    // Debug function to print current time
    public void DebugCurrentTime()
    {
        Debug.Log($"[TimerUI] Current Time: {currentTime:F2}");
    }

public float GetRemainingTime() => currentTime;
}
