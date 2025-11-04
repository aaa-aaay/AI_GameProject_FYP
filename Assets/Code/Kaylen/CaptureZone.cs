using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CaptureZone : MonoBehaviour
{
    [Header("References")]
    public CaptureCheck captureManager;

    private void Start()
    {
        if (captureManager == null)
            captureManager = FindFirstObjectByType<CaptureCheck>();

        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detect player only (your player uses "Runner" tag)
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            // Check if player is holding a rabbit
            Runner held = player.GetHeldRunner();
            if (held != null)
            {
                // Add to capture count
                if (captureManager != null)
                    captureManager.RunnerCaptured();

                // Drop and destroy the rabbit
                player.DropRunner();
                Destroy(held.gameObject);

                Debug.Log($"Runner {held.name} captured automatically.");
            }
        }
    }
}
