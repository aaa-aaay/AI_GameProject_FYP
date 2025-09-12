using System;
using UnityEngine;

public class LobbyPlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    private InputManager _inputManager;


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
    }

    private void HandleMoveEnd()
    {
        _moveInput = Vector2.zero;
    }

    private void FixedUpdate()
    {
        Vector3 move = new Vector3(_moveInput.x, 0, _moveInput.y) * _moveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(transform.position + transform.TransformDirection(move));
    }


}
