using System;
using Unity.VisualScripting;
using UnityEngine;

public class RaceCarControl : MonoBehaviour
{

    private RaceCarMovement _movement;
    private InputManager _inputManager;
    private Vector2 _moveInput;
    private bool _isDrifting;

 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _movement = GetComponent<RaceCarMovement>();
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _inputManager.OnMove += HandleMove;
        _inputManager.OnMoveEnd += HandleMoveEnd;
        _inputManager.onDash += HandleDrift;
        _inputManager.onDashEnd += HandleDriftEnd;

    }

    private void OnDestroy()
    {
        _inputManager.OnMove -= HandleMove;
    }

    // Update is called once per frame

    private void HandleMove(Vector2 direction)
    {
        _moveInput = direction;
    }

    private void HandleDrift()
    {
        if(Mathf.Abs(_moveInput.x) > 0.1) //moving left or right
        {
            _isDrifting = true;
        }
    }

    private void HandleDriftEnd()
    {
        _isDrifting = false;
    }

    private void HandleMoveEnd()
    {
        _moveInput = Vector3.zero;
    }

    private void FixedUpdate()
    {
        _movement.MoveCar(_moveInput,_isDrifting);
        
    }
}
