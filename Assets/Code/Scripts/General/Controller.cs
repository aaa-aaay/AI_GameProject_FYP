using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    [SerializeField] private InputActionReference left_click;
    [SerializeField] private InputActionReference move;

    [SerializeField] private UnityEvent<InputAction.CallbackContext> ClickedLeft;
    [SerializeField] private UnityEvent<InputAction.CallbackContext> StartMove;
    [SerializeField] private UnityEvent<InputAction.CallbackContext> EndMove;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        left_click.action.started += InvokeClickedLeft;
        move.action.started += InvokeStartMove;
        move.action.canceled += InvokeEndMove;
    }

    private void InvokeClickedLeft(InputAction.CallbackContext context)
    {
        if (ClickedLeft != null)
        {
            ClickedLeft.Invoke(context);
        }
    }

    private void InvokeStartMove(InputAction.CallbackContext context)
    {
        if (StartMove != null)
        {
            StartMove.Invoke(context);
        }
    }

    private void InvokeEndMove(InputAction.CallbackContext context)
    {
        if (EndMove != null)
        {
            EndMove.Invoke(context);
        }
    }
}
