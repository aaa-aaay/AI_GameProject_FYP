using UnityEngine;

public class BadmintionPlayer : MonoBehaviour
{

    [SerializeField] private float _moveSpeed = 5f;
    private RacketSwing _racketSwing;



    private Rigidbody _rb;  
    private Vector2 _moveInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _racketSwing = GetComponent<RacketSwing>();


        InputManager inputManager = ServiceLocator.Instance.GetService<InputManager>();


        //Movements
        inputManager.OnMove += HandleMove;
        inputManager.OnMoveEnd += HandleMoveEnd;
        inputManager.OnJump += HandleJump;

        //racket "Attacks"
        inputManager.OnClick += HandleClick;
        inputManager.onRightClick += HandleRightClick;
        inputManager.onMiddleClick += HandleMiddleClick;


    }


    private void HandleMove(Vector2 direction)
    {
        // Store input from InputManager
        _moveInput = direction;
    }

    private void HandleMoveEnd()
    {
        _moveInput = Vector2.zero;
    }

    private void HandleJump()
    {
        // Add jump force if grounded (simplest version)
        if (Mathf.Abs(_rb.linearVelocity.y) < 0.01f)
        {
            _rb.AddForce(Vector3.up, ForceMode.Impulse);
        }
    }


    private void HandleClick()
    {
        if (_racketSwing.racketSwinging) return;
        _racketSwing.StartSwing(Racket.ShotType.Lob);
    }
    private void HandleRightClick()
    {
        if (_racketSwing.racketSwinging) return;
        _racketSwing.StartSwing(Racket.ShotType.Drop);
    }
    private void HandleMiddleClick()
    {
        if (_racketSwing.racketSwinging) return;
        _racketSwing.StartSwing(Racket.ShotType.Smash);
    }



    private void FixedUpdate()
    {
        // Move front/back using z-axis (W/S or up/down)
        Vector3 move = new Vector3(_moveInput.x, 0, _moveInput.y) * _moveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(transform.position + transform.TransformDirection(move));
    }
}
