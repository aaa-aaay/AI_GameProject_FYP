using System.Collections.Generic;
using UnityEngine;

public class InfScrollMainMenu : MonoBehaviour
{
    [SerializeField] private Transform _camera;     // Or camera
    [SerializeField] private float segmentLength = 50f;
    [SerializeField] private int numberOfSegments = 3;
    [SerializeField] private GameObject roadPrefab;

    private Queue<GameObject> activeSegments = new Queue<GameObject>();
    private float spawnZ = 0f;

    private void Start()
    {
        for (int i = 0; i < numberOfSegments; i++)
        {
            SpawnSegment();
        }
    }

    private void Update()
    {
        if (_camera.position.z - segmentLength > activeSegments.Peek().transform.position.z)
        {
            // Move the oldest segment to the front
            GameObject movedSegment = activeSegments.Dequeue();
            movedSegment.transform.position = Vector3.forward * spawnZ;
            spawnZ += segmentLength;
            activeSegments.Enqueue(movedSegment);
        }
    }

    private void SpawnSegment()
    {
        GameObject segment = Instantiate(roadPrefab, Vector3.forward * spawnZ, Quaternion.identity);
        activeSegments.Enqueue(segment);
        spawnZ += segmentLength;
    }
}
