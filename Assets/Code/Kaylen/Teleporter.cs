using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Header("Teleport Target")]
    public Transform targetPosition; 

    [Header("UI Reference")]
    public GameObject captureText; 

    private bool hasActivated = false;

    private void Start()
    {
        
        if (captureText != null)
            captureText.SetActive(false);
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

          
            if (captureText != null)
            {
                captureText.SetActive(true);
            }

            hasActivated = true;
        }
    }
}
