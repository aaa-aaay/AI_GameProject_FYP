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
    public Material newModel; // new material after capture
    public List<Transform> taggerSpawnPoints; // where taggers spawn (can be <3)
    public GameObject taggerPrefab; // tagger prefab to spawn

    private PlayerMovement playerMovement;

    private void Start()
    {
        // Use FindObjectOfType for compatibility across Unity versions
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("CaptureManager: No PlayerMovement found in the scene!");
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
        // Wait one frame so any tag coroutine finishes its current frame (safe)
        yield return null;

        if (playerMovement == null) yield break;

        Debug.Log("Capture threshold reached — executing transition.");

        // 1) Disable player's tagging immediately to avoid race conditions
        playerMovement.DisableTagging();

        // 2) Teleport player
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

        // 3) Spawn 3 taggers (or fewer if spawn points list is small). If no spawn points, spawn near player.
        if (taggerPrefab == null)
        {
            Debug.LogWarning("CaptureManager: taggerPrefab is not assigned. No taggers will be spawned.");
        }
        else
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
                    // fallback: spawn around the player's new position
                    spawnPos = playerMovement.transform.position + Random.insideUnitSphere * 2f;
                    spawnPos.y = playerMovement.transform.position.y;
                }

                GameObject spawned = Instantiate(taggerPrefab, spawnPos, spawnRot);
                if (!spawned.activeSelf) spawned.SetActive(true);
                Debug.Log($"Spawned tagger at {spawnPos}");
            }
        }

        // 4) Change player's material (try GetComponentInChildren if Renderer is on child)
        if (newModel != null && playerMovement.playerModel != null)
        {
            Renderer rend = playerMovement.playerModel.GetComponent<Renderer>();
            if (rend == null) rend = playerMovement.playerModel.GetComponentInChildren<Renderer>();

            if (rend != null)
            {
                rend.material = newModel;
                Debug.Log("Player material changed.");
            }
            else
            {
                Debug.LogWarning("CaptureManager: Could not find Renderer on playerModel to change material.");
            }
        }
    }
}
