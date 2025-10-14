using System;
using Unity.VisualScripting;
using UnityEngine;

public class RaceCarControl : MonoBehaviour
{

    private InputManager _inputManager;
    private Rigidbody _rb;
    private Vector2 _moveInput;

    [SerializeField] private float speed = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _inputManager.OnMove += HandleMove;

    }

    private void OnDestroy()
    {
        _inputManager.OnMove -= HandleMove;
    }

    // Update is called once per frame
    void Update()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void HandleMove(Vector2 direction)
    {
        _moveInput = direction;
    }

    private void FixedUpdate()
    {

        _rb.MovePosition(transform.position + new Vector3(_moveInput.x,0, _moveInput.y) * speed * Time.deltaTime);

    }
}
