﻿using UnityEngine;
using UnityEngine.InputSystem;

public class archery_player : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    private bool isTurn;

    private InputActionMap archeryMap;
    private InputAction move;
    private InputAction increaseForce;
    private InputAction decreaseForce;
    private InputAction shoot;

    private float minForce;
    private float maxForce;
    private float maxYaw;
    private float minPitch;
    private float maxPitch;

    private archery_handler handler;
    private archery_settings settings;

    public float force { get; private set; }
    public float yaw { get; private set; }
    public float pitch { get; private set; }

    public void Initialize()
    {
        isTurn = false;
        handler = archery_handler.instance;
        settings = handler.settings;

        minForce = settings.minForce;
        maxForce = settings.maxForce;
        maxYaw = settings.maxYaw;
        minPitch = settings.minPitch;
        maxPitch = settings.maxPitch;

        foreach (var map in inputActions.actionMaps) map.Disable();
        archeryMap = inputActions.FindActionMap("Archery", true);
        move = archeryMap.FindAction("Move", true);
        increaseForce = archeryMap.FindAction("IncreaseForce", true);
        decreaseForce = archeryMap.FindAction("DecreaseForce", true);
        shoot = archeryMap.FindAction("Shoot", true);

        if (maxForce < minForce)
        {
            Debug.LogWarning("maxForce (" + maxForce + ") less than minForce (" + minForce + "). maxForce set to minForce.");
            maxForce = minForce;
        }
        
        if (maxPitch < minPitch)
        {
            Debug.LogWarning("maxPitch (" + maxPitch + ") less than minPitch (" + minPitch + "). maxPitch set to minPitch.");
            maxPitch = minPitch;
        }

        StartTurn();
    }

    public void StartTurn()
    {
        isTurn = true;
        archeryMap.Enable();

        if (minForce > 10f) force = minForce; else force = 10f;
        yaw = 0f;
        if (minPitch > 0f) pitch = minPitch; else pitch = 0f;
    }

    private void Update()
    {
        if (!isTurn) return;

        Vector2 moveInput = move.ReadValue<Vector2>();

        yaw += moveInput.x * 10f * Time.deltaTime;
        pitch += moveInput.y * 10f * Time.deltaTime;

        yaw = Mathf.Clamp(yaw, -maxYaw, maxYaw);
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        if (increaseForce.IsPressed())
            force = Mathf.Min(force + 10f * Time.deltaTime, maxForce);

        if (decreaseForce.IsPressed())
            force = Mathf.Max(force - 10f * Time.deltaTime, minForce);

        if (shoot.triggered)
        {
            handler.Shoot(force, yaw, pitch);
            isTurn = false;
            archeryMap.Disable();
        }

        handler.UpdateUI(force, yaw, pitch);
    }
}