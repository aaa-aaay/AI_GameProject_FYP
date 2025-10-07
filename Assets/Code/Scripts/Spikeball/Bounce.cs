using UnityEngine;

public class Bounce : MonoBehaviour
{
    [SerializeField] private float force_multiplier = 0.25f;
    [SerializeField] private LayerMask bounce_layers;

    private Rigidbody rigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (rigidbody != null)
        {
            if ((bounce_layers & (1 << collision.gameObject.layer)) != 0)
            {
                Vector3 vel = rigidbody.linearVelocity;
                vel.y = 0;
                rigidbody.AddForce(Vector3.up * vel.magnitude * force_multiplier, ForceMode.Impulse);
            }
        }
    }
}
