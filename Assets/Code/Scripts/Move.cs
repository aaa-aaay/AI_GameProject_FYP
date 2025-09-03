using UnityEngine;
using UnityEngine.InputSystem;

public class Move : MonoBehaviour
{
    [SerializeField] private float movespeed;
    [SerializeField] private float rotation_time;

    private Vector3 direction;
    private float time_passed;

    private void Start()
    {
        direction = Vector3.zero;
        time_passed = 0;
    }

    private void Update()
    {
        if (direction != Vector3.zero)
        {
            time_passed += Time.deltaTime;

            if (time_passed > rotation_time)
            {
                time_passed = rotation_time;
            }
            
            transform.forward = Vector3.Lerp(transform.forward, direction, time_passed / rotation_time);
            transform.position += direction * Time.deltaTime * movespeed;
        }
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
}
