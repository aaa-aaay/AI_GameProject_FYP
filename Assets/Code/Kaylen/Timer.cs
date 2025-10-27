using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TimerUI : MonoBehaviour
{
    [Header("Timer Settings")]
    public float maxTime = 30f;    // Total time for the episode
    public TMP_Text timerText;     // Reference to UI Text

    [Header("Exit UI")]
    public CanvasGroup exitCanvasGroup;   // Use CanvasGroup for fade
    public float fadeDuration = 1f;       // Fade duration
    public float displayDuration = 3f;    // Time before fade out

    [Header("Exit Spawn Settings")]
    public GameObject exitPrefab;         // The prefab to spawn
    public List<Transform> spawnPoints;   // List of possible spawn points

    private float currentTime;
    private bool isRunning = false;
    private bool hasTriggeredExit = false;

    void Start()
    {
        ResetTimer();

        if (exitCanvasGroup != null)
            exitCanvasGroup.alpha = 0f; // hide at start
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;
        timerText.text = Mathf.Ceil(currentTime).ToString();

        if (currentTime <= 0f && !hasTriggeredExit)
        {
            currentTime = 0f;
            isRunning = false;
            hasTriggeredExit = true;

            ShowExitCanvas();
            SpawnExit();
        }
    }

    void ShowExitCanvas()
    {
        if (exitCanvasGroup == null)
        {
            Debug.LogWarning("Exit CanvasGroup is not assigned!");
            return;
        }

        StopAllCoroutines();
        StartCoroutine(FadeExitCanvas());
    }

    IEnumerator FadeExitCanvas()
    {
        // Fade in
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            exitCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        exitCanvasGroup.alpha = 1f;

        // Wait for display duration
        yield return new WaitForSeconds(displayDuration);

        // Fade out
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

        // Pick a random spawn point
        Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        // Spawn the prefab
        Instantiate(exitPrefab, randomPoint.position, randomPoint.rotation);
        Debug.Log("Exit spawned at: " + randomPoint.name);
    }

    public void StartTimer()
    {
        ResetTimer();
        isRunning = true;
        hasTriggeredExit = false;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        currentTime = maxTime;
        timerText.text = Mathf.Ceil(currentTime).ToString();
        hasTriggeredExit = false;

        if (exitCanvasGroup != null)
            exitCanvasGroup.alpha = 0f;
    }

    public float GetRemainingTime()
    {
        return currentTime;
    }
}
