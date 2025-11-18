using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CircleAnimationController : MonoBehaviour
{
    [SerializeField] private Vector3 rotation_values;
    [SerializeField] private float base_speed;
    [SerializeField] private float moving_speed;

    private Animator animator;

    private bool moving;
    private float speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();

        moving = false;
        speed = base_speed;
    }

    private void Update()
    {
        transform.rotation = transform.rotation * Quaternion.Euler(rotation_values * Time.deltaTime * speed);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moving = context.ReadValue<Vector2>() != Vector2.zero;
        if (!moving)
        {
            speed = base_speed;
        }
        else
        {
            speed = moving_speed;
        }
        animator.SetBool("Moving", moving);
    }
}
