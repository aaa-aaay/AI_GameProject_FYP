using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public string playerTag = "Player"; // Tag of player allowed to pick up the key
    private ExitTrigger exitDoor;       // Reference to ExitTrigger
    private bool collected = false;

    [Header("Floating Settings")]
    public float bobAmplitude = 0.25f;  // Height of bobbing motion
    public float bobSpeed = 2f;         // Speed of bobbing
    private Vector3 startPos;           // Original position of the key
    private Camera mainCamera;          // Cached main camera reference

    // UI Key Indicator
    private GameObject keyUI;

    private void Start()
    {
        // Cache starting position and main camera
        startPos = transform.position;
        mainCamera = Camera.main;

        // Find the ExitTrigger in the scene
        exitDoor = FindFirstObjectByType<ExitTrigger>();
        if (exitDoor == null)
            Debug.LogWarning("[KeyPickup] No ExitTrigger found in the scene!");

        // --- UI KEY ICON SETUP ---
        keyUI = GameObject.Find("Key");   // looks for a GameObject named "Key"
        if (keyUI != null)
        {
            keyUI.SetActive(false);       // hide until key is picked up
        }
        else
        {
            Debug.LogWarning("[KeyPickup] No UI Key object named 'Key' found in scene.");
        }
    }

    private void Update()
    {
        // Floating (bobbing) effect
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

            // Mark player's star condition
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.pickedUpKey = true;
                Debug.Log("[KeyPickup] Player star condition: keyCollected = true");
            }

            // Unlock exit door
            if (exitDoor != null)
            {
                exitDoor.UnlockDoor();
            }

            // --- ENABLE UI ICON HERE ---
            if (keyUI != null)
            {
                keyUI.SetActive(true);
            }

            Destroy(gameObject);
        }
    }
}
