using UnityEngine;

public class RotateRigidbodyForce : MonoBehaviour
{
    Rigidbody body;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        transform.forward = body.linearVelocity;
    }
}
