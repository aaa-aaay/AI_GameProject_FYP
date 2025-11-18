using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public string playerTag = "Player";
    private ExitTrigger exitDoor;
    private bool collected = false;
    public GameObject keyUI;

    [Header("Floating Settings")]
    public float bobAmplitude = 0.25f;
    public float bobSpeed = 2f;
    private Vector3 startPos;
    private Camera mainCamera;

   

    private void Start()
    {
        keyUI = GameObject.Find("Key");
        if (keyUI == null)
            Debug.LogWarning("[KeyPickup] Could not auto-assign KeyUI! Make sure an object named 'KeyUI' exists in the scene.");
        else
            keyUI.SetActive(false); 

        startPos = transform.position;
        mainCamera = Camera.main;

        exitDoor = FindFirstObjectByType<ExitTrigger>();
        if (exitDoor == null)
            Debug.LogWarning("[KeyPickup] No ExitTrigger found in the scene!");
    }

    private void Update()
    {
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

            // --- SHOW UI ICON ---
            if (keyUI != null)
                keyUI.SetActive(true);
            else
                Debug.LogWarning("[KeyPickup] No Key UI assigned!");

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
            else
            {
                Debug.LogWarning("[KeyPickup] Tried to unlock ExitDoor, but none was found!");
            }

            Destroy(gameObject);
        }
    }
}
