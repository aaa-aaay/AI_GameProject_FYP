using System;
using UnityEngine;

public class WallFrictionHandler : MonoBehaviour
{

    [Header("Crash into wall")]
    [SerializeField] private float _impactToCountAsCrash = 5.0f;
    [SerializeField] [Range(0,1)] private float _endVelocityRatio = 0.5f;
    [SerializeField][Range(0, 1)] private float _onWallVelocityRatio = 0.95f;
    [SerializeField] private string wallTagName= "Wall";

    public event Action OnHitWall;
    public event Action OnWallStay;

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
            if (impact > _impactToCountAsCrash) // tweak threshold
            {
                // Drop velocity (instant deceleration)
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.linearVelocity *= _endVelocityRatio; // or even lower

                OnHitWall?.Invoke();
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag(wallTagName))
        {
            _rb.linearVelocity *= _onWallVelocityRatio;
            OnWallStay?.Invoke();
        }
    }
}
