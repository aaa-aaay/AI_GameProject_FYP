using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

[DefaultExecutionOrder(-1)]
public class InputManager : MonoBehaviour,IGameService
{

    [SerializeField]
    private InputActionAsset _inputActionAsset;

    [SerializeField]
    private InputActionReference _moveActionReference;

    [SerializeField]
    private InputActionReference _jumpActionReference;


    [SerializeField] 
    InputActionReference _clickActionReference;


    [SerializeField]
    InputActionReference _rightClickActionReference;


    [SerializeField]
    private InputActionReference _middleClickReference;

    [SerializeField]
    private InputActionReference _interactActionReference;

    [SerializeField]
    private InputActionReference _dashActionReference;


    public event Action<Vector2> OnMove;
    public event Action OnMoveEnd;

    public event Action OnJump;

    public event Action OnClick;

    public event Action onRightClick;
    public event Action onMiddleClick;

    public event Action onInteract;
    public event Action onDash;

    private void Awake()
    {
        ServiceLocator.Instance.AddService(this, false);
    }
    private void OnEnable()
    {
        _inputActionAsset.Enable();

        BindActions();

    }
    private void OnDisable()
    {
        //Debug.Log("InputManager OnDisable called");
        //UnbindActions();

        //_inputActionAsset.Disable();



    }

    private void OnDestroy()
    {
        //ServiceLocator.Instance.RemoveService<InputManager>(false);
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
        Debug.Log("Jumped");
    }

    private void Click(InputAction.CallbackContext context)
    {
        OnClick?.Invoke();
    }

    private void RightClick(InputAction.CallbackContext context)
    {
        onRightClick?.Invoke();
    }

    private void MiddleClick(InputAction.CallbackContext context)
    {
        onMiddleClick?.Invoke();
    }

    private void Interact(InputAction.CallbackContext context)
    {
        
        Debug.Log("Interact action fired: " + context.phase);
        onInteract?.Invoke();
    }

    private void Dash(InputAction.CallbackContext context)
    {
        onDash?.Invoke();
    }



    private void BindActions()
    {
        _moveActionReference.action.performed += Move;
        _moveActionReference.action.canceled += MoveEnd;
        _jumpActionReference.action.performed += Jump;
        _clickActionReference.action.performed += Click;
        _rightClickActionReference.action.performed += RightClick;
        _middleClickReference.action.performed += MiddleClick;
        _interactActionReference.action.performed += Interact;
        _dashActionReference.action.performed += Dash;
    }

    private void UnbindActions()
    {
        _moveActionReference.action.performed -= Move;
        _moveActionReference.action.canceled -= MoveEnd;

        _jumpActionReference.action.performed -= Jump;
        _clickActionReference.action.performed -= Click;
        _rightClickActionReference.action.performed -= RightClick;
        _middleClickReference.action.performed -= MiddleClick;
        _interactActionReference.action.performed -= Interact;
    }

    public void EnableActions()
    {
        _moveActionReference.action.Enable();
        _jumpActionReference.action.Enable();
        _clickActionReference.action.Enable();
        _rightClickActionReference.action.Enable();
        _middleClickReference.action.Enable();
        _interactActionReference.action.Enable();
        _dashActionReference.action.Enable();
    }
}
