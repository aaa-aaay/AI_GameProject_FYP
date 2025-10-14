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

      
        if (timerUI != null)
            timerUI.UpdateCaptureCount(currentCaptures, captureAmount);

        if (currentCaptures >= captureAmount)
        {
            StartCoroutine(HandleCaptureThresholdReached());
            currentCaptures = 0;
        }
    }


    private IEnumerator HandleCaptureThresholdReached() //Coroutine for progression
    {
        yield return null;

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
            {
                rend.material = newModel;
               
            }
           
        }

       
        playerMovement.DisableTagging();

    
        if (nextMapSpawnPoint != null)
        {
            playerMovement.transform.position = nextMapSpawnPoint.position;
            playerMovement.transform.rotation = nextMapSpawnPoint.rotation;
            Debug.Log($"Player teleported to {nextMapSpawnPoint.position}");
        }
       

       
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
     
    }
}
