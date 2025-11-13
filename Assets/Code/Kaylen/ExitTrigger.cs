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
    public TimerUI timer;       // Reference to timer
    public KeyPickup keyPickup; // Reference to key object (optional — auto-found)

    private bool isUnlocked = false;
    private bool isOpen = false;
    private bool isRotating = false;
    private bool hasWon = false;

    private void Start()
    {
        // Auto-assign references if missing
        if (timer == null)
        {
            timer = FindFirstObjectByType<TimerUI>();
            if (timer == null)
                Debug.LogWarning("[ExitTrigger] No TimerUI found in the scene!");
        }

        if (keyPickup == null)
        {
            keyPickup = FindFirstObjectByType<KeyPickup>();
            if (keyPickup == null)
                Debug.LogWarning("[ExitTrigger] No KeyPickup found in the scene!");
        }

        // Ensure this collider is NOT a trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = false;
    }

    private void Update()
    {
        // Only open when timer is done AND key is collected
        if (!isUnlocked && timer != null)
        {
            if (timer.GetRemainingTime() <= 0f && IsKeyCollected())
            {
                UnlockDoor();
            }
        }
    }

    public bool IsKeyCollected()
    {
        // Consider key collected if it's destroyed or missing
        return keyPickup == null;
    }

    public void UnlockDoor()
    {
        if (!isUnlocked)
        {
            isUnlocked = true;
            Debug.Log("Door unlocked!");

            if (!isOpen && !isRotating)
                StartCoroutine(OpenDoor());
        }
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
            Debug.Log("You win!");
            // Add your win logic here (scene transition, UI, etc.)
        }
    }
}
