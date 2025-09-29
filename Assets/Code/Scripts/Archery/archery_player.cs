using UnityEngine;
using UnityEngine.InputSystem;

public class archery_player : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    private InputActionMap archeryMap;
    private InputAction move;
    private InputAction increaseForce;
    private InputAction decreaseForce;
    private InputAction shoot;

    [Header("Physics")]
    [SerializeField, Range(5f, Mathf.Infinity)] private float minForce = 10f;
    [SerializeField, Range(5f, Mathf.Infinity)] private float maxForce = 30f;
    [SerializeField, Range(-45f, 45f)] private float maxYaw = 30f;
    [SerializeField, Range(-90f, 90f)] private float minPitch = 0f;
    [SerializeField, Range(-90f, 90f)] private float maxPitch = 30f;

    public float force { get; private set; }
    public float yaw { get; private set; }
    public float pitch { get; private set; }
    public float windStrength { get; private set; }
    public float windDirection { get; private set; }

    private void OnEnable()
    {
        foreach (var map in inputActions.actionMaps) map.Disable();
        archeryMap = inputActions.FindActionMap("Archery", true);
        archeryMap.Enable();
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
    }

    private void OnDisable()
    {
        archeryMap.Disable();
    }

    public void Init(float strength, float direction)
    {
        if (minForce > 10f) force = minForce; else force = 10f;
        yaw = 0f;
        if (minPitch > 0f) pitch = minPitch; else pitch = 0f;

        windStrength = strength;
        windDirection = direction;
    }

    private void Update()
    {
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
            archery_handler.instance.Shoot(force, yaw, pitch);
    }
}