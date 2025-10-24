using System;
using Unity.VisualScripting;
using UnityEngine;

public class RaceCarControl : MonoBehaviour
{
    [SerializeField] RaceManager _manager;
    [SerializeField] GameObject _car;

    private ResetCarPosition _carPosResetter;
    private GoalChecker _goalChecker;
    private BetterCarMovement _movement;
    private InputManager _inputManager;
    private Vector2 _moveInput;
    private bool _isDrifting;

 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _manager.onRaceOver += RestartRace;
        _movement = GetComponent<BetterCarMovement>();
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _inputManager.OnMove += HandleMove;
        _inputManager.OnMoveEnd += HandleMoveEnd;
        _inputManager.onDash += HandleDrift;
        _inputManager.onDashEnd += HandleDriftEnd;


        _carPosResetter = _car.GetComponent<ResetCarPosition>();
        _goalChecker = _car.GetComponent<GoalChecker>();

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
        if(_isDrifting) return;


        if (Mathf.Abs(_moveInput.x) > 0.1) //moving left or right
        {
            _isDrifting = true;
            _movement.ToggleDrifting(_isDrifting, _moveInput.x);
            Debug.Log("caleed");

        }
    }

    private void HandleDriftEnd()
    {
        _isDrifting = false;
        _movement.ToggleDrifting(_isDrifting);
    }

    private void HandleMoveEnd()
    {
        _moveInput = Vector3.zero;
    }


    private void Update()
    {
        _movement.MoveCar(_moveInput);
    }


    private void RestartRace()
    {
        _carPosResetter.ResetPos();
        _goalChecker.ResetCar();
    }
}
