using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TimerUI : MonoBehaviour
{
    public float maxTime = 30f;       // Total time for the episode
    public TMP_Text timerText;            // Reference to UI Text

    private float currentTime;
    private bool isRunning = false;

    void Start()
    {
        ResetTimer();
        StartTimer(); // starts counting automatically
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;
        timerText.text = Mathf.Ceil(currentTime).ToString();

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;
        }
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        currentTime = maxTime;
        timerText.text = Mathf.Ceil(currentTime).ToString();
    }

    public float GetRemainingTime()
    {
        return currentTime;
    }
}
