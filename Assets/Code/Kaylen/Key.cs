using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public string playerTag = "Runner"; // Player tag to detect
    private ExitTrigger exitDoor;       // Auto-found ExitTrigger in Start()
    private bool collected = false;

    [Header("Floating Settings")]
    public float bobAmplitude = 0.25f;  // Height of bobbing motion
    public float bobSpeed = 2f;         // Speed of bobbing
    private Vector3 startPos;           // Original position of the object
    private Camera mainCamera;          // Cached reference to main camera

    private void Start()
    {
        // Cache starting position and main camera
        startPos = transform.position;
        mainCamera = Camera.main;

        // Find the first ExitTrigger in the scene
        exitDoor = FindFirstObjectByType<ExitTrigger>();
        if (exitDoor == null)
            Debug.LogWarning("[KeyPickup] No ExitTrigger found in the scene!");
    }

    private void Update()
    {
        // Face toward the camera (billboarding)
        if (mainCamera != null)
        {
            Vector3 direction = mainCamera.transform.position - transform.position;
            direction.y = 0f; // Keep upright (no tilting)
            transform.rotation = Quaternion.LookRotation(-direction);
        }

        // Bob up and down
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (other.CompareTag(playerTag))
        {
            collected = true;
            Debug.Log("[KeyPickup] Key collected!");

            // Unlock or trigger exit door if found
            if (exitDoor != null)
            {
                exitDoor.UnlockDoor();
            }
            else
            {
                Debug.LogWarning("[KeyPickup] Tried to unlock ExitDoor, but none was found!");
            }

            Destroy(gameObject);
        }
    }
}
