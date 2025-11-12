using System;
using UnityEngine;

public class LobbyPlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 20f;
    [SerializeField] private Animator _animator;
    private InputManager _inputManager;
    private bool forceStop = false;

    private Vector2 _moveInput;
    private Rigidbody _rb;
    private void Start()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();


        //Movements
        _inputManager.OnMove += HandleMove;
        _inputManager.OnMoveEnd += HandleMoveEnd;
        //inputManager.OnJump += HandleJump;

        //racket "Attacks"
        //inputManager.OnClick += HandleClick;
        //inputManager.onRightClick += HandleRightClick;
        //inputManager.onMiddleClick += HandleMiddleClick;


        _rb = GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        _inputManager.OnMove -= HandleMove;
        _inputManager.OnMoveEnd -= HandleMoveEnd;
    }

    private void HandleMove(Vector2 direction)
    {
        _moveInput = direction;
        _animator.SetBool("walking", true);
    }

    private void HandleMoveEnd()
    {
        
        _moveInput = Vector2.zero;
        _rb.linearVelocity = new Vector3(0,0,0);
        _animator.SetBool("walking", false);
    }

    private void FixedUpdate()
    {

        if (forceStop) return;
        Vector3 move = new Vector3(_moveInput.x, 0, _moveInput.y);

        if (move.sqrMagnitude > 0.001f)
        {

            Vector3 moveDir = move.normalized;
            Vector3 targetVelocity = moveDir * _moveSpeed;
            targetVelocity.y = _rb.linearVelocity.y; // Keep gravity

            _rb.linearVelocity = targetVelocity;

            // Rotate smoothly toward movement direction
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            Quaternion smoothedRotation = Quaternion.Slerp(_rb.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);
            _rb.MoveRotation(smoothedRotation);
        }
    }

    public void ForceIdle(bool start)
    {
        forceStop = start;
        _animator.SetBool("walking", forceStop);
    }



}
