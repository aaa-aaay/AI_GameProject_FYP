using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagHitbox : MonoBehaviour
{
    [Tooltip("Assigned at spawn time by PlayerMovement.DoTag()")]
    public PlayerMovement owner;

    // allow only one pickup per runner collision
    private HashSet<GameObject> processed = new HashSet<GameObject>();

    // lifetime fallback (set by PlayerMovement too, but safe guard)
    public float autoDestroyAfter = 0.6f;

    void Start()
    {
        // safety auto-destruct
        Destroy(gameObject, autoDestroyAfter);
    }

    private void TryPickup(Collider other)
    {
        if (!other.CompareTag("Runner")) return;
        if (owner == null)
        {
            Debug.LogWarning("TagHitbox: owner is null, cannot pickup.");
            return;
        }

        GameObject go = other.gameObject;
        if (processed.Contains(go)) return;

        processed.Add(go);

        Runner runner = other.GetComponent<Runner>();
        if (runner == null)
        {
            Debug.LogWarning($"TagHitbox: {other.name} has Runner tag but no RunnerAgent component.");
            return;
        }

        // notify CaptureManager if you still want counting (optional)
        CaptureManager cm = FindFirstObjectByType<CaptureManager>();
        if (cm != null)
        {
            // if you want to count capture when picked up, uncomment:
            // cm.RunnerCaptured();
        }

        owner.PickUpRunner(runner);
        Debug.Log($"TagHitbox: picked up runner {runner.name}");

        // destroy hitbox immediately on pickup
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryPickup(other);
    }

    // fallback so a brief overlap won't miss the pickup if physics timing is odd
    private void OnTriggerStay(Collider other)
    {
        TryPickup(other);
    }

    private void OnDestroy()
    {
        processed.Clear();
    }
}
