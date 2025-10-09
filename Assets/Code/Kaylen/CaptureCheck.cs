using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureManager : MonoBehaviour
{
    [Header("Capture Settings")]
    public int captureAmount = 3; // how many captures until next map
    private int currentCaptures = 0;

    [Header("Next Map Settings")]
    public Transform nextMapSpawnPoint;  // where to teleport player
    public List<Transform> taggerSpawnPoints; // where taggers spawn (can be <3)
    public GameObject taggerPrefab; // tagger prefab to spawn

    [Header("Visual Settings")]
    public Material newModel; // 👈 material to apply after capture

    [Header("Timer Reference")]
    public TimerUI timerUI; // reference to the UI timer

    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("CaptureManager: No PlayerMovement found in the scene!");
        }

        if (timerUI == null)
        {
            timerUI = FindFirstObjectByType<TimerUI>();
        }
    }

    public void RunnerCaptured()
    {
        currentCaptures++;
        Debug.Log($"Runner captured. ({currentCaptures}/{captureAmount})");

        if (currentCaptures >= captureAmount)
        {
            StartCoroutine(HandleCaptureThresholdReached());
            currentCaptures = 0;
        }
    }

    private IEnumerator HandleCaptureThresholdReached()
    {
        yield return null;

        if (playerMovement == null) yield break;

        Debug.Log("Capture threshold reached — executing transition.");

        // ✅ Start the timer immediately when capture requirement is met
        if (timerUI != null)
        {
            timerUI.StartTimer();
            Debug.Log("Timer started due to capture threshold.");
        }
        else
        {
            Debug.LogWarning("CaptureManager: TimerUI reference missing — timer not started.");
        }

        // ✅ Change player material (independent of timer)
        if (newModel != null && playerMovement.playerModel != null)
        {
            Renderer rend = playerMovement.playerModel.GetComponent<Renderer>();
            if (rend == null) rend = playerMovement.playerModel.GetComponentInChildren<Renderer>();

            if (rend != null)
            {
                rend.material = newModel;
                Debug.Log("Player material changed (independent of timer).");
            }
            else
            {
                Debug.LogWarning("CaptureManager: Could not find Renderer on playerModel to change material.");
            }
        }

        // Disable tagging to prevent interactions during transition
        playerMovement.DisableTagging();

        // Teleport player
        if (nextMapSpawnPoint != null)
        {
            playerMovement.transform.position = nextMapSpawnPoint.position;
            playerMovement.transform.rotation = nextMapSpawnPoint.rotation;
            Debug.Log($"Player teleported to {nextMapSpawnPoint.position}");
        }
        else
        {
            Debug.LogWarning("CaptureManager: nextMapSpawnPoint is not assigned.");
        }

        // Spawn new taggers
        if (taggerPrefab != null)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector3 spawnPos;
                Quaternion spawnRot = Quaternion.identity;

                if (i < taggerSpawnPoints.Count && taggerSpawnPoints[i] != null)
                {
                    spawnPos = taggerSpawnPoints[i].position;
                    spawnRot = taggerSpawnPoints[i].rotation;
                }
                else
                {
                    spawnPos = playerMovement.transform.position + Random.insideUnitSphere * 2f;
                    spawnPos.y = playerMovement.transform.position.y;
                }

                GameObject spawned = Instantiate(taggerPrefab, spawnPos, spawnRot);
                if (!spawned.activeSelf) spawned.SetActive(true);
                Debug.Log($"Spawned tagger at {spawnPos}");
            }
        }
        else
        {
            Debug.LogWarning("CaptureManager: taggerPrefab is not assigned. No taggers will be spawned.");
        }
    }
}
