using UnityEngine;
using UnityEngine.UI;

public class Bounce : MonoBehaviour
{
    [SerializeField] private float bounce_force;
    [SerializeField] private float wall_bounce_force;

    [SerializeField] private LayerMask bounce_layers;

    private Rigidbody rigid_body;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid_body = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (rigid_body != null)
        {
            if ((bounce_layers & (1 << collision.gameObject.layer)) != 0)
            {
                Vector3 temp = rigid_body.linearVelocity;
                temp.y = 0;

                rigid_body.AddForce(new Vector3(0, temp.magnitude, 0) * bounce_force);
            }  
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (rigid_body != null)
        {
            if ((bounce_layers & (1 << collision.gameObject.layer)) == 0)
            {
                Vector3 temp = rigid_body.linearVelocity;
                temp.y = 0;

                rigid_body.AddForce(temp.normalized * wall_bounce_force, ForceMode.Impulse);
            }
        }
    }
}
