using UnityEngine;
using System.Collections.Generic;

public class Teleporter : MonoBehaviour
{
    [Header("Teleport Target")]
    public Transform targetPosition; // Where the runner will be teleported

    [Header("Prefab Spawning")]
    public GameObject prefabToSpawn;              // Prefab you want to spawn
    public List<Transform> spawnPoints = new();   // List of positions to spawn the prefab

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering is the Runner
        if (other.CompareTag("Runner"))
        {
            // Teleport Runner
            if (targetPosition != null)
            {
                other.transform.position = targetPosition.position;
                Debug.Log($"{other.name} teleported to {targetPosition.position}");
            }

            // Spawn prefabs at each spawn point
            if (prefabToSpawn != null && spawnPoints.Count > 0)
            {
                foreach (Transform spawnPoint in spawnPoints)
                {
                    Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation); 
                    Debug.Log($"Spawned prefab at {spawnPoint.position}");
                }
            }
            else
            {
                Debug.LogWarning("Prefab or spawn points not assigned!");
            }
        }
    }
}
