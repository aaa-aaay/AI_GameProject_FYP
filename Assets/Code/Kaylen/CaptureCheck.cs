using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureCheck : MonoBehaviour
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
    public List<GameObject> objectsToHide;
    public List<GameObject> objectsToShow;
    private PlayerMovement playerMovement;

    [Header("Fade Screen")]
    public CanvasGroup fadeScreen;
    public float fadeDuration = 1.5f;
    public float blackHoldDuration = 3f;

    private bool movementWasDisabled = false;

    void Start()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        if (playerMovement == null)
            Debug.LogError("CaptureCheck: No PlayerMovement found in the scene!");

        if (timerUI == null)
            timerUI = FindFirstObjectByType<TimerUI>();

        if (fadeScreen != null)
            fadeScreen.alpha = 0f;
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

        DisablePlayerMovement(true);
        playerMovement.DisableTagging();

        // Apply new model if any
        if (newModel != null && playerMovement.playerModel != null)
        {
            Renderer rend = playerMovement.playerModel.GetComponent<Renderer>();
            if (rend == null) rend = playerMovement.playerModel.GetComponentInChildren<Renderer>();
            if (rend != null)
                rend.material = newModel;
        }

        // Teleport player
        if (nextMapSpawnPoint != null)
        {
            playerMovement.transform.position = nextMapSpawnPoint.position;
            playerMovement.transform.rotation = nextMapSpawnPoint.rotation;
            Debug.Log($"Player teleported to {nextMapSpawnPoint.position}");
        }

        // Manage visibility
        foreach (var obj in objectsToHide)
            if (obj != null) obj.SetActive(false);

        foreach (var obj in objectsToShow)
            if (obj != null) obj.SetActive(true);

        // Fade transition
        if (fadeScreen != null)
        {
            Debug.Log("Fading to black...");
            yield return StartCoroutine(FadeToBlack());
            yield return new WaitForSeconds(blackHoldDuration);
            Debug.Log("Fading back in...");
            yield return StartCoroutine(FadeFromBlack());
        }

        // Spawn taggers after fade
        yield return StartCoroutine(SpawnTaggers());

        // Re-enable player & start timer
        DisablePlayerMovement(false);

        if (timerUI != null)
        {
            timerUI.StartTimer();
            Debug.Log("Timer started after taggers spawned");
        }
    }

    private void DisablePlayerMovement(bool disable)
    {
        if (playerMovement == null) return;

        // Try to find a known flag or fallback to enabling/disabling component
        var moveComponent = playerMovement.GetComponent<PlayerMovement>();
        if (moveComponent != null)
        {
            moveComponent.enabled = !disable;
            movementWasDisabled = disable;
        }
    }

    private IEnumerator FadeToBlack()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeScreen.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }
        fadeScreen.alpha = 1f;
    }

    private IEnumerator FadeFromBlack()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeScreen.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }
        fadeScreen.alpha = 0f;
    }

    private IEnumerator SpawnTaggers()
    {
        if (taggerPrefab == null) yield break;

        Debug.Log("Spawning taggers...");

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

            if (Physics.Raycast(spawnPos + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 20f))
                spawnPos = hit.point + Vector3.up * 0.1f;
            else
                Debug.LogWarning($"Tagger {i} spawn raycast missed ground, using default Y height");

            GameObject spawned = Instantiate(taggerPrefab, spawnPos, spawnRot);
            spawned.SetActive(true);
            Debug.Log($"Spawned tagger {i + 1} at {spawnPos}");
            yield return null;
        }
    }
}
