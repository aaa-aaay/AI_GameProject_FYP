using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour,IGameService
{

    [SerializeField]
    private InputActionAsset _inputActionAsset;

    [SerializeField]
    private InputActionReference _moveActionReference;

    [SerializeField]
    private InputActionReference _jumpActionReference;


    [SerializeField] InputActionReference _clickActionReference;

    public event Action<Vector2> OnMove;
    public event Action OnMoveEnd;

    public event Action OnJump;

    public event Action OnClick;



    private void OnEnable()
    {
        ServiceLocator.Instance.AddService(this, false);


        _inputActionAsset.Enable();
        _moveActionReference.action.Enable();
        _moveActionReference.action.performed += Move;
        _moveActionReference.action.canceled += MoveEnd;

        _jumpActionReference.action.Enable();
        _jumpActionReference.action.performed += Jump;

        _clickActionReference.action.Enable();
        _clickActionReference.action.performed += Click;

    }
    private void OnDisable()
    {
        ServiceLocator.Instance.RemoveService<InputManager>(false);


        _inputActionAsset.Disable();
        _moveActionReference.action.Disable();
        _moveActionReference.action.performed -= Move;
        _moveActionReference.action.canceled -= MoveEnd;

        _jumpActionReference.action.Disable();
        _jumpActionReference.action.performed -= Jump;


    }


    private void Move(InputAction.CallbackContext context)
    {
        OnMove?.Invoke(context.ReadValue<Vector2>());
    }

    private void MoveEnd(InputAction.CallbackContext context)
    {
        OnMoveEnd?.Invoke();
    }

    private void Jump(InputAction.CallbackContext context)
    {
        OnJump?.Invoke();
    }

    private void Click(InputAction.CallbackContext context)
    {
        OnClick?.Invoke();
    }
}
