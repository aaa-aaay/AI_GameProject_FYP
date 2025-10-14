using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TimerUI : MonoBehaviour
{
    [Header("Timer Settings")]
    public float maxTime = 30f;  
    public TMP_Text timerText;    
    public TMP_Text captureText;
    [Header("Exit UI")]
    public CanvasGroup exitCanvasGroup;  
    public float fadeDuration = 1f;      
    public float displayDuration = 3f;   

    [Header("Exit Spawn Settings")]
    public GameObject exitPrefab;       
    public List<Transform> spawnPoints;   

    private float currentTime;
    private bool isRunning = false;
    private bool hasTriggeredExit = false;
    private bool showingTimer = false;
    void Start()
    {
        ResetTimer();

        if (exitCanvasGroup != null)
            exitCanvasGroup.alpha = 0f;

   
        ShowCaptureCount();
    }

    void Update()
    {
        if (!isRunning || !showingTimer) return;

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
    public void UpdateCaptureCount(int current, int total)
    {
        ShowCaptureCount();
        if (captureText != null)
            captureText.text = $"Captured: {current} / {total}";
    }

    public void ShowCaptureCount()
    {
        showingTimer = false;
        if (timerText != null) timerText.gameObject.SetActive(false);
        if (captureText != null) captureText.gameObject.SetActive(true);
    }
    public void ShowTimer()
    {
        showingTimer = true;
        if (captureText != null) captureText.gameObject.SetActive(false);
        if (timerText != null) timerText.gameObject.SetActive(true);
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

    IEnumerator FadeExitCanvas() //Coroutine to fade in
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

    public float GetRemainingTime() => currentTime;
}
