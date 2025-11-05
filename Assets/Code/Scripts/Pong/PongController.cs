using UnityEngine;
using UnityEngine.InputSystem;

public class PongController : MonoBehaviour
{
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       animator = GetComponent<Animator>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        animator.SetBool("Moving", context.ReadValue<Vector2>() != Vector2.zero);
    }

    public void OnMove(Vector2 direction)
    {
        animator.SetBool("Moving", direction != Vector2.zero);
    }
}
