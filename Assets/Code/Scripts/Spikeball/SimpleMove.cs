using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleMove : MonoBehaviour
{
    [SerializeField] protected float movespeed;

    protected Vector3 direction;
    protected Rigidbody rigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        direction = Vector3.zero;
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rigidbody.linearVelocity = direction * movespeed;
    }

    public virtual void set_direction(InputAction.CallbackContext context)
    {
        Vector3 new_direction = Vector3.zero;
        new_direction.x = context.ReadValue<Vector2>().x;
        new_direction.z = context.ReadValue<Vector2>().y;

        if (direction != new_direction)
        {
            direction = new_direction;
            direction.Normalize();
        }
    }

    public virtual void set_direction(Vector2 new_direction)
    {
        Vector3 temp_direction = Vector3.zero;
        temp_direction.x = new_direction.x;
        temp_direction.z = new_direction.y;

        if (direction != temp_direction)
        {
            direction = temp_direction;
            direction.Normalize();
        }
    }
}
