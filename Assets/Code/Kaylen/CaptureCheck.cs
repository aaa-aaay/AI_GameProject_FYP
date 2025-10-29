using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureManager : MonoBehaviour
{
    [Header("Capture Settings")]
    public int captureAmount = 3;
    private int currentCaptures = 0;

    [Header("Next Map Settings")]
    public Transform nextMapSpawnPoint;
    public List<Transform> taggerSpawnPoints;
    public GameObject taggerPrefab;

    [Header("Visual Settings")]
    public Material newModel;

    [Header("Timer Reference")]
    public TimerUI timerUI;

    [Header("Object Visibility Control")]
    public List<GameObject> objectsToHide;  // hide when threshold reached
    public List<GameObject> objectsToShow;  // show when threshold reached
    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        if (playerMovement == null)
            Debug.LogError("CaptureManager: No PlayerMovement found in the scene!");

        if (timerUI == null)
            timerUI = FindFirstObjectByType<TimerUI>();
    }

    public void RunnerCaptured()
    {
        currentCaptures++;
        Debug.Log($"Runner captured. ({currentCaptures}/{captureAmount})");
        FindFirstObjectByType<RabbitUI>()?.OnRunnerCaptured();

  

        if (currentCaptures >= captureAmount)
        {
            StartCoroutine(HandleCaptureThresholdReached());
            currentCaptures = 0;
        }
    }

    private IEnumerator HandleCaptureThresholdReached()
    {


        if (playerMovement == null) yield break;

        Debug.Log("Capture threshold reached");

        if (timerUI != null)
        {
            timerUI.StartTimer();
            Debug.Log("Timer started");
        }

        if (newModel != null && playerMovement.playerModel != null)
        {
            Renderer rend = playerMovement.playerModel.GetComponent<Renderer>();
            if (rend == null) rend = playerMovement.playerModel.GetComponentInChildren<Renderer>();

            if (rend != null)
                rend.material = newModel;
        }

        playerMovement.DisableTagging();

        if (nextMapSpawnPoint != null)
        {
            playerMovement.transform.position = nextMapSpawnPoint.position;
            playerMovement.transform.rotation = nextMapSpawnPoint.rotation;
            Debug.Log($"Player teleported to {nextMapSpawnPoint.position}");
        }
        foreach (var obj in objectsToHide)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        foreach (var obj in objectsToShow)
        {
            if (obj != null)
                obj.SetActive(true);
        }

        // ✅ FIXED: Safe spawn for taggers (raycast to terrain)
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
                    spawnPos.y = playerMovement.transform.position.y + 2f;
                }

                // ✅ Raycast downward to ensure it’s above ground
                if (Physics.Raycast(spawnPos + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 20f))
                {
                    spawnPos = hit.point + Vector3.up * 0.1f; // lift slightly above the ground
                }
                else
                {
                    Debug.LogWarning($"Tagger {i} spawn raycast missed ground, using default Y height");
                }

                GameObject spawned = Instantiate(taggerPrefab, spawnPos, spawnRot);
                spawned.SetActive(true);
                Debug.Log($"Spawned tagger at {spawnPos}");
            }
        }
    }
}
