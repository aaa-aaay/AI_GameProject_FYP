using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ExitTrigger : MonoBehaviour
{
    [Header("Door Rotation Settings")]
    public Transform doorObject;
    public float openAngle = 120f;
    public float rotationSpeed = 60f;
    public Vector3 rotationAxis = Vector3.up;

    [Header("References")]
    public TimerUI timer;
    public KeyPickup keyPickup;

    [Header("Star Conditions")]
    public bool keyCollected = false;   // 1 star condition
    public bool escaped = false;        // 2 stars condition
    public bool perfectEscape = false;  // 3 stars condition

    private bool isUnlocked = false;
    private bool isOpen = false;
    private bool isRotating = false;
    private bool hasWon = false;

    private void Start()
    {
        if (timer == null)
            timer = FindFirstObjectByType<TimerUI>();

        if (keyPickup == null)
            keyPickup = FindFirstObjectByType<KeyPickup>();

        Collider col = GetComponent<Collider>();
        col.isTrigger = false;
    }

    private void Update()
    {
        // Only unlock when timer is done AND key collected
        if (!isUnlocked && timer != null)
        {
            if (timer.GetRemainingTime() <= 0f && IsKeyCollected())
                UnlockDoor();
        }

        // Continuously track key collected status
        keyCollected = IsKeyCollected();
    }

    public bool IsKeyCollected()
    {
        // Consider key collected if destroyed or missing
        return keyPickup == null;
    }

    public void UnlockDoor()
    {
        if (isUnlocked) return;
        isUnlocked = true;
        Debug.Log("Door unlocked!");

        if (!isOpen && !isRotating)
            StartCoroutine(OpenDoor());
    }

    private IEnumerator OpenDoor()
    {
        isRotating = true;
        float startAngle = doorObject.localEulerAngles.y;
        float endAngle = openAngle;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * (rotationSpeed / Mathf.Abs(endAngle - startAngle));
            float angle = Mathf.LerpAngle(startAngle, endAngle, t);
            doorObject.localRotation = Quaternion.Euler(rotationAxis * angle);
            yield return null;
        }

        doorObject.localRotation = Quaternion.Euler(rotationAxis * endAngle);
        isRotating = false;
        isOpen = true;
        Debug.Log("Door opened.");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasWon && isOpen && (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Runner")))
        {
            hasWon = true;
            escaped = true;

            PlayerMovement player = collision.collider.GetComponent<PlayerMovement>();
            if (player != null)
            {
                keyCollected = player.pickedUpKey;
                perfectEscape = player.HasPerfectRun();
            }

            Debug.Log($"You win! Stars earned: {CalculateStars()}");
            // Add your win logic here (scene transition, UI, etc.)
        }
    }

    // Helper: Determine how many stars earned
    public int CalculateStars()
    {
        int stars = 0;
        if (keyCollected) stars++;
        if (escaped) stars++;
        if (perfectEscape) stars++;
        return stars;
    }
}
