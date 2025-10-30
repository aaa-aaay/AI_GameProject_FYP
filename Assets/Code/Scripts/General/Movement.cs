using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] protected float movespeed;

    protected Vector3 direction;
    protected Vector3 last_direction;

    protected Rigidbody rigid_body;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MovementStart();
    }

    protected void MovementStart()
    {
        direction = Vector3.zero;
        last_direction = direction;

        rigid_body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveUpdate();
    }

    protected void MoveUpdate()
    {
        if (rigid_body != null)
        {
            rigid_body.linearVelocity = direction * movespeed;
        }
    }

    public virtual void SetDirection(Vector3 new_direction)
    {
        last_direction = direction;
        direction = new_direction.normalized;
    }

    public void SetDirection(InputAction.CallbackContext context)
    {
        SetDirection(context.ReadValue<Vector2>());
    }

    public void SetDirectionTopdown(InputAction.CallbackContext context)
    {
        Vector2 temp = context.ReadValue<Vector2>();
        SetDirection(new Vector3(temp.x, 0, temp.y));
    }

    public void SetDirectionTopDown(Vector2 new_direction)
    {
        SetDirection(new Vector3(new_direction.x, 0, new_direction.y));
    }

    public virtual void AddDirection(Vector3 added_direction)
    {
        direction += added_direction.normalized;
        direction.Normalize();
    }

    public void AddDirection(InputAction.CallbackContext context)
    {
        AddDirection(context.ReadValue<Vector2>());
    }

    public void AddDirectionTopDown(InputAction.CallbackContext context)
    {
        Vector2 temp = context.ReadValue<Vector2>();
        AddDirection(new Vector3(temp.x, 0, temp.y));
    }

    public void AddDirectionTopDown(Vector2 new_direction)
    {
        AddDirection(new Vector3(new_direction.x, 0, new_direction.y));
    }

    public void ResetDirection()
    {
        direction = Vector3.zero;
    }

    public void ResetDirection(InputAction.CallbackContext context)
    {
        ResetDirection();
    }

    public void SetDirectionHorizontal(Vector2 new_direction)
    {
        direction.x = new_direction.x;
        direction.y = 0;
        direction.z = 0;
        direction.Normalize();
    }

    public void SetDirectionHorizontal(InputAction.CallbackContext context)
    {
        SetDirectionHorizontal(context.ReadValue<Vector2>());
    }
}
