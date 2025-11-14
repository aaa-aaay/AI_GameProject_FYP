using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureCheck : MonoBehaviour
{


    [Header("Areas")]
    [SerializeField] private GameObject area1;
    [SerializeField] private GameObject area2;
    [Header("Capture Settings")]
    public int captureAmount = 3;
    private int currentCaptures = 0;

    [Header("Next Map Settings")]
    public ExitTrigger exitTrigger;
    public Transform nextMapSpawnPoint;
    public List<Transform> taggerSpawnPoints;
    public GameObject taggerPrefab;

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
    public float playerHeadStartTime = 3f;

    [Header("Pen Animation Settings")]
    public Transform penDoor;
    public float rotationDuration = 1.5f;
    public Transform penSpawnPoint;
    public GameObject penSpawnPrefab;

    [Header("Post-Capture Spawn Settings")]
    public GameObject postCapturePrefab;             // Prefab to spawn after all captures
    public List<Transform> postCaptureSpawnPoints;   // Spawn positions for that prefab

    private bool hasOpenedPen = false;
    private bool movementWasDisabled = false;

    private float captureTimer = 0;
    private bool stillCapturing = false;

    void Start()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        if (playerMovement == null)
            Debug.LogError("CaptureCheck: No PlayerMovement found in the scene!");

        if (timerUI == null)
            timerUI = FindFirstObjectByType<TimerUI>();

        if (fadeScreen != null)
            fadeScreen.alpha = 0f;

        // Ensure the door starts closed at 120° Y
        if (penDoor != null)
        {
            Vector3 angles = penDoor.localEulerAngles;
            angles.y = 120f;
            penDoor.localEulerAngles = angles;
        }
        area1.SetActive(true);
        area2.SetActive(false);
        stillCapturing = true;
        captureTimer = 0;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("[DEBUG] P pressed – Forcing capture threshold sequence...");
            StopAllCoroutines();
            StartCoroutine(HandleCaptureThresholdReached());
        }

        if (stillCapturing) captureTimer += Time.deltaTime;
    }
    public void RunnerCaptured()
    {
        currentCaptures++;
        Debug.Log($"Runner captured. ({currentCaptures}/{captureAmount})");

        FindFirstObjectByType<RabbitUI>()?.OnRunnerCaptured();

        // OPEN PEN ON FIRST CAPTURE ONLY
        if (!hasOpenedPen && penDoor != null)
        {
            hasOpenedPen = true;
            StartCoroutine(RotateDoorSmooth(penDoor, 120f, 0f, rotationDuration));
        }

        // ALWAYS SPAWN PEN PREFAB EACH TIME A RUNNER IS CAPTURED
        if (penSpawnPrefab != null && penSpawnPoint != null)
        {
            Instantiate(penSpawnPrefab, penSpawnPoint.position, penSpawnPoint.rotation);
            Debug.Log("Spawned pen prefab at spawn point.");
        }

        // CHECK CAPTURE THRESHOLD
        if (currentCaptures >= captureAmount)
        {
            StartCoroutine(HandleCaptureThresholdReached());
            currentCaptures = 0;
        }
    }

    private IEnumerator RotateDoorSmooth(Transform door, float startY, float endY, float duration)
    {
        float elapsed = 0f;

        // Start at specified angle
        Vector3 startEuler = door.localEulerAngles;
        startEuler.y = startY;
        door.localEulerAngles = startEuler;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            float newY = Mathf.Lerp(startY, endY, t);
            Vector3 angles = door.localEulerAngles;
            angles.y = newY;
            door.localEulerAngles = angles;
            yield return null;
        }

        Vector3 finalAngles = door.localEulerAngles;
        finalAngles.y = endY;
        door.localEulerAngles = finalAngles;

        Debug.Log("Pen door finished opening (120 → 0).");
    }

    private IEnumerator HandleCaptureThresholdReached()
    {
        if (playerMovement == null) yield break;

        Debug.Log("Capture threshold reached.");
        stillCapturing = false;
        DisablePlayerMovement(true);
        area1.SetActive(false);
        area2.SetActive(true);
        playerMovement.DisableTagging();


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

        // Fade to black
        if (fadeScreen != null)
        {
            Debug.Log("Fading to black...");
            yield return StartCoroutine(FadeToBlack());
        }

        // Spawn taggers while screen is black
        yield return StartCoroutine(SpawnTaggers());

        // Hold black screen briefly
        yield return new WaitForSeconds(blackHoldDuration);

        // Fade back in
        if (fadeScreen != null)
        {
            Debug.Log("Fading back in...");
            yield return StartCoroutine(FadeFromBlack());
        }

        // ✅ Spawn only one post-capture prefab at a random spawn point
        if (postCapturePrefab != null && postCaptureSpawnPoints != null && postCaptureSpawnPoints.Count > 0)
        {
            Transform chosenPoint = postCaptureSpawnPoints[Random.Range(0, postCaptureSpawnPoints.Count)];
            if (chosenPoint != null)
            {
                Instantiate(postCapturePrefab, chosenPoint.position, chosenPoint.rotation);
                Debug.Log($"Spawned ONE post-capture prefab at {chosenPoint.position}");
            }
        }

        // Re-enable movement
        DisablePlayerMovement(false);

        if (timerUI != null)
        {
            timerUI.StartTimer();
            Debug.Log("Timer started after fade-in.");
        }

        // Player head start
        Debug.Log($"Player head start for {playerHeadStartTime} seconds...");
        yield return new WaitForSeconds(playerHeadStartTime);

        // Enable taggers
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
                Debug.LogWarning($"Tagger {i} spawn raycast missed ground, using default Y height.");

            GameObject spawned = Instantiate(taggerPrefab, spawnPos, spawnRot);
            spawned.SetActive(true);

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
        Debug.Log("All taggers activated after head start.");
    }
}
