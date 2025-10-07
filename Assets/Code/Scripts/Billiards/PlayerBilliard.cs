using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBilliard : MonoBehaviour
{
    [SerializeField] private InputActionReference action;
    [SerializeField] private float turn_scale;
    [SerializeField] private float force_percentage;

    private float turn_value;
    private float force_value;

    private void Start()
    {
        turn_value = 0;
        force_value = 0f;

        action.action.started += turn;
        action.action.canceled += turn;
    }

    private void Update()
    {
        transform.rotation = transform.rotation * Quaternion.Euler(0, turn_value * turn_scale * Time.deltaTime, 0);
        force_percentage += force_value * Time.deltaTime;
        force_percentage = Mathf.Clamp(force_percentage, 0, 1);
    }

    public void shoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            BilliardSingleton.instance.shoot_ball(gameObject, transform.forward , force_percentage);
        }
    }

    public void turn(InputAction.CallbackContext context)
    {
        Vector2 temp = context.ReadValue<Vector2>();
        turn_value = temp.x;
        force_value = temp.y;
        print("Started");
    }

    public void end_turn(InputAction.CallbackContext context)
    {
        turn_value = 0;
        force_value = 0;
        print("Ended");
    }
}
