using UnityEngine;

public class WallFrictionHandler : MonoBehaviour
{

    [Header("Crash into wall")]
    [SerializeField] private float _impactToCountAsCrash = 5.0f;
    [SerializeField] [Range(0,1)] private float _endVelocityRatio = 0.5f;
    [SerializeField][Range(0, 1)] private float _onWallVelocityRatio = 0.95f;
    [SerializeField] private string wallTagName= "Wall";

    private Rigidbody _rb;
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(wallTagName))
        {
            float impact = collision.relativeVelocity.magnitude;
            Debug.Log("Impact: " +  impact);
            if (impact > 5f) // tweak threshold
            {
                // Drop velocity (instant deceleration)
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.linearVelocity *= 0.5f; // or even lower
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag(wallTagName))
        {
            _rb.linearVelocity *= _onWallVelocityRatio;
        }
    }
}
