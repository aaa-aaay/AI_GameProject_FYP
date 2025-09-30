using UnityEngine;
using System.Collections.Generic;

public class Teleporter : MonoBehaviour
{
    [Header("Teleport Target")]
    public Transform targetPosition; // Where the runner will be teleported 

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

           
        }
    }
}
