using UnityEngine;
using UnityEngine.InputSystem;

public class BoxingController : MonoBehaviour
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

    public void OnPunch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetBool("Punch", true);
        }
    }

    public void ResetPunch()
    {
        animator.SetBool("Punch", false);
    }
}
