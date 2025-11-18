using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class archery_player : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    private archery_handler handler;
    private archery_settings settings;

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

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip aimAnim;
    [SerializeField] private PositionConstraint stringConstraint;
    [SerializeField] private Transform spine;
    [SerializeField] private Transform bow;
    [SerializeField] private Transform arrow;

    private Vector3 spineRotation;
    private Vector3 stringOrigin;

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

        spineRotation = spine.rotation.eulerAngles;
        stringOrigin = stringConstraint.transform.localPosition;
    }

    public void Ready()
    {
        animator.enabled = true;
        animator.SetTrigger("ReadyAim");
    }

    public void StartTurn()
    {
        isTurn = true;
        archeryMap.Enable();
        stringConstraint.constraintActive = true;
        arrow.gameObject.SetActive(true);

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
            handler.Shoot(bow.position, force, yaw, pitch);
            isTurn = false;
            archeryMap.Disable();
            stringConstraint.constraintActive = false;
            stringConstraint.transform.localPosition = stringOrigin;
            arrow.gameObject.SetActive(false);

            animator.enabled = true;
            animator.SetTrigger("Shot");

            return;
        }

        handler.UpdateUI(bow.position, force, yaw, pitch);

        animator.enabled = true;
        float normalizedForce = Mathf.InverseLerp(minForce, maxForce, force);
        animator.Play(aimAnim.name, 0, normalizedForce);
        animator.Update(0f);
        animator.enabled = false;

        spine.localRotation = Quaternion.Euler(spineRotation.x, spineRotation.y + 20f + yaw, spineRotation.z - pitch);
    }
}