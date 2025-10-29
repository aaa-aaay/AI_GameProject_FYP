using UnityEngine;
using System.Collections.Generic;

public class Teleporter : MonoBehaviour
{
    [Header("Teleport Target")]
    public Transform targetPosition;

    [Header("Objects to Show/Hide")]
    public List<GameObject> objectsToActivate; // List of objects to activate

    private bool hasActivated = false;

    private void Start()
    {
        // Ensure everything starts hidden if desired
        foreach (var obj in objectsToActivate)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasActivated) return;

        if (other.CompareTag("Runner"))
        {
            // Teleport Runner
            if (targetPosition != null)
            {
                other.transform.position = targetPosition.position;
                Debug.Log($"{other.name} teleported to {targetPosition.position}");
            }

            // Activate all assigned objects
            foreach (var obj in objectsToActivate)
            {
                if (obj != null)
                    obj.SetActive(true);
            }

         
            hasActivated = true;
        }
    }
}
