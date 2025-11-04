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

    [Header("Tagger Settings")]
    public float playerHeadStartTime = 3f; // seconds before taggers start moving

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

        // Fade to black first
        if (fadeScreen != null)
        {
            Debug.Log("Fading to black...");
            yield return StartCoroutine(FadeToBlack());
        }

        // Spawn taggers while screen is black
        yield return StartCoroutine(SpawnTaggers());

        // Keep screen black for a bit (blackHoldDuration)
        yield return new WaitForSeconds(blackHoldDuration);

        // Fade back in
        if (fadeScreen != null)
        {
            Debug.Log("Fading back in...");
            yield return StartCoroutine(FadeFromBlack());
        }

        // Re-enable player movement and start timer immediately
        DisablePlayerMovement(false);

        if (timerUI != null)
        {
            timerUI.StartTimer();
            Debug.Log("Timer started after fade-in");
        }

        // Give player head start before taggers move
        Debug.Log($"Player head start for {playerHeadStartTime} seconds...");
        yield return new WaitForSeconds(playerHeadStartTime);

        // Enable taggers after head start
        EnableAllTaggers();
    }


    private void DisablePlayerMovement(bool disable)
    {
        if (playerMovement == null) return;

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

            // Temporarily disable movement or AI
            var ai = spawned.GetComponent<TaggerAgent>();
            if (ai != null)
                ai.enabled = false;

            Debug.Log($"Spawned tagger {i + 1} at {spawnPos}");
            yield return null;
        }
    }

    private void EnableAllTaggers()
    {
        TaggerAgent[] taggers = FindObjectsByType<TaggerAgent>(FindObjectsSortMode.None);
        foreach (var tagger in taggers)
        {   
            tagger.enabled = true;
        }
        Debug.Log("All taggers activated after head start");
    }

}
