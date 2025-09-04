using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    [SerializeField] private float movespeed;
    [SerializeField] private float dash_force;
    [SerializeField] private float rotation_time;
    [SerializeField] private float dash_cooldown;

    private Rigidbody rigidbody;
    private Vector3 direction;
    private Vector3 last_direction;
    private float time_passed;
    private float dash_time_passed;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        direction = Vector3.zero;
        last_direction = transform.forward;
        time_passed = 0;
        dash_time_passed = dash_cooldown;
    }

    private void Update()
    {
        dash_time_passed += Time.deltaTime;

        if (direction != Vector3.zero)
        {
            last_direction = direction;
            transform.position += transform.forward * Time.deltaTime * movespeed;
        }
        else
        {
            EventHandler.InvokePunish(gameObject, 0.01f);
        }

            time_passed += Time.deltaTime;

        if (time_passed > rotation_time)
        {
            time_passed = rotation_time;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(last_direction), time_passed / rotation_time);
    }

    public void set_direction(Vector3 new_direction)
    {
        if (direction != new_direction)
        {
            time_passed = 0;
            direction = new_direction;
            direction.Normalize();
        }
    }

    public void set_direction(InputAction.CallbackContext context)
    {
        Vector3 new_direction = Vector3.zero;
        new_direction.x = context.ReadValue<Vector2>().x;
        new_direction.z = context.ReadValue<Vector2>().y;

        if (direction != new_direction)
        {
            time_passed = 0;
            direction = new_direction;
            direction.Normalize();
        }
    }

    public void set_direction(Vector2 new_direction)
    {
        Vector3 temp_direction = Vector3.zero;
        temp_direction.x = new_direction.x;
        temp_direction.z = new_direction.y;

        if (direction != temp_direction)
        {
            time_passed = 0;
            direction = temp_direction;
            direction.Normalize();
        }
    }

    public void dash()
    {
        if (dash_time_passed >= dash_cooldown)
        {
            rigidbody.AddForce(last_direction * dash_force, ForceMode.Impulse);
            dash_time_passed = 0;
        }
    }

    public void dash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            dash();
        }
    }
}
