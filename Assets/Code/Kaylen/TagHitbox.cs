using UnityEngine;
using System.Collections.Generic;

public class TagHitbox : MonoBehaviour
{
    // Keep track of which runners were already tagged this frame
    private HashSet<GameObject> taggedRunners = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Runner"))
        {
            Debug.Log($"[TagHitbox] Ignored non-runner: {other.name}");
            return;
        }

        // Skip if this Runner was already tagged
        if (taggedRunners.Contains(other.gameObject))
        {
            Debug.Log($"[TagHitbox] Ignored duplicate collider from {other.gameObject.name}");
            return;
        }

        taggedRunners.Add(other.gameObject);

        Debug.Log($"[TagHitbox] Runner captured: {other.gameObject.name} (First hit)");

        // Notify capture manager
        CaptureManager cm = FindFirstObjectByType<CaptureManager>();
        if (cm != null)
        {
            cm.RunnerCaptured();
        }

        Destroy(other.gameObject);
    }

    private void OnDestroy()
    {
        // Clear references to prevent memory leaks
        taggedRunners.Clear();
    }
}
