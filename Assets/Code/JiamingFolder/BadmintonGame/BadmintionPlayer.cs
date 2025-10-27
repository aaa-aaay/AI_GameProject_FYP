using System.Runtime.CompilerServices;
using UnityEngine;

public class BadmintionPlayer : MonoBehaviour
{

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _slowedMoveSpeed = 5f;
    [SerializeField] private float _evenMoreSlowedSpeed = 5f;
    private RacketSwing _racketSwing;
    private BadmintionMovement _movement;
    private BadmintonStamina _stamina;
    //private BadmintonStamina _stamina;

    private int _shootingDirection;

    private Rigidbody _rb;  
    private Vector2 _moveInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _racketSwing = GetComponent<RacketSwing>();
        _movement = GetComponent<BadmintionMovement>();
        _stamina = GetComponent<BadmintonStamina>();
        InputManager inputManager = ServiceLocator.Instance.GetService<InputManager>();


        //Movements
        inputManager.OnMove += HandleMove;
        inputManager.OnMoveEnd += HandleMoveEnd;

        //racket "Attacks"
        inputManager.OnClick += HandleClick;
        inputManager.onRightClick += HandleRightClick;
        inputManager.onMiddleClick += HandleMiddleClick;
        inputManager.onDash += HandleDash;


    }

    private void OnDestroy()
    {
        InputManager inputManager = ServiceLocator.Instance.GetService<InputManager>();


        //Movements
        inputManager.OnMove -= HandleMove;
        inputManager.OnMoveEnd -= HandleMoveEnd;

        //racket "Attacks"
        inputManager.OnClick -= HandleClick;
        inputManager.onRightClick -= HandleRightClick;
        inputManager.onMiddleClick -= HandleMiddleClick;
        inputManager.onDash -= HandleDash;
    }


    private void HandleMove(Vector2 direction)
    {
        // Store input from InputManager
        _moveInput = direction;
        _movement.Walk(true);
        if (_moveInput.x < 0) _shootingDirection = 1;
        else if (_moveInput.x > 0) _shootingDirection = 2;

    }

    private void HandleMoveEnd()
    {

        _moveInput = Vector2.zero;
        _movement.Walk(false);

    }

    private void HandleClick()
    {
        WantToSwingRacket(Racket.ShotType.Lob, BadmintonStamina.actions.lobShot);
    }
    private void HandleRightClick()
    {
        WantToSwingRacket(Racket.ShotType.Drop, BadmintonStamina.actions.DropShot);
    }
    private void HandleMiddleClick()
    {
        WantToSwingRacket(Racket.ShotType.Smash, BadmintonStamina.actions.SmashShot);
    }
    private void HandleDash()
    {
        
        if (_stamina.UseStamina(BadmintonStamina.actions.Dash))
        _movement.Dash(new Vector3(_moveInput.x, 0, _moveInput.y));
    }

    private void WantToSwingRacket(Racket.ShotType shotType, BadmintonStamina.actions action)
    {
        if (_racketSwing.racketSwinging) return;
        //_stamina.UseStamina(action);
        _racketSwing.StartSwing(shotType, _shootingDirection);
    }



    private void FixedUpdate()
    {
        if (_moveInput == Vector2.zero) _stamina.UseStamina(BadmintonStamina.actions.Rest);
        else _stamina.UseStamina(BadmintonStamina.actions.Running);

        float finalMoveSpeed = _moveSpeed;

        if (_stamina.GetStamina() < 50)
        {
            finalMoveSpeed = _slowedMoveSpeed;
            //decrease speed;
        }
        if (_stamina.GetStamina() < 25)
        {

            finalMoveSpeed = _evenMoreSlowedSpeed;
            //decrease speed even more

        }

        Vector3 move = new Vector3(_moveInput.x, 0, _moveInput.y) * finalMoveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(transform.position + transform.TransformDirection(move));
    }
}
