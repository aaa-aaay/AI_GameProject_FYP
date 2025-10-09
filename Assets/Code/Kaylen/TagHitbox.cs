using UnityEngine;

public class TagHitbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Runner"))
        {
            // Notify capture manager
            CaptureManager cm = FindFirstObjectByType<CaptureManager>();
            if (cm != null) cm.RunnerCaptured();

            // If Runner is an ML-Agent, consider calling EndEpisode() on its script instead
            Destroy(other.gameObject);
        }
    }
}
