using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Jump : MonoBehaviour
{
    [SerializeField] private LayerMask raycast_layers;

    [SerializeField] private float jump_force;

    private Rigidbody rigid_body;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid_body = GetComponent<Rigidbody>();
    }

    public void JumpUp()
    {
        if (CanJump())
        {
            rigid_body.AddForce(Vector3.up * jump_force, ForceMode.Impulse);
        }
    }

    public bool CanJump()
    {
        return Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, 0.11f, raycast_layers);
    }

    public void JumpUp(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            JumpUp();
        }
    }
}
