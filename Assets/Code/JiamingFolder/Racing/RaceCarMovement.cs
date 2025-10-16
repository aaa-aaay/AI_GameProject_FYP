using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RaceCarMovement : MonoBehaviour
{
    private Rigidbody _rb;

    [Header("Arcade Movement Settings")]
    [SerializeField] private float _acceleration = 50f;
    [SerializeField] private float _reverseAcceleration = 30f;
    [SerializeField] private float _maxSpeed = 25f;
    [SerializeField] private float _rotationSpeed = 120f;
    [SerializeField] private float _driftTurnBoost = 1.5f;
    [SerializeField] private float _brakeDecel = 10f;

    [Header("Physics Tweaks")]
    [SerializeField] private float _groundRayLength = 2.0f;
    [SerializeField] private float _traction = 8f;
    [SerializeField] private float _driftTraction = 4f;
    [SerializeField] private float _downforce = 10f;

    private bool _isGrounded;
    private float _currentSpeed;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.mass = 800f;
        _rb.linearDamping = 0.2f;
        _rb.angularDamping = 2f;
        _rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    public void MoveCar(Vector2 inputDir, bool isDrifting)
    {
        float forwardInput = inputDir.y;
        float turnInput = inputDir.x;

        _isGrounded = GroundCheck();
        if (!_isGrounded) return;

        _currentSpeed = Vector3.Dot(_rb.linearVelocity, transform.forward);

        // --- 1. Acceleration ---
        if (forwardInput > 0.1f)
        {
            _rb.AddForce(transform.forward * forwardInput * _acceleration, ForceMode.Acceleration);
        }
        else if (forwardInput < -0.1f)
        {
            _rb.AddForce(transform.forward * forwardInput * _reverseAcceleration, ForceMode.Acceleration);
        }
        else
        {
            // Apply friction when no input
            _rb.linearVelocity = Vector3.Lerp(_rb.linearVelocity, Vector3.zero, Time.deltaTime * _brakeDecel);
        }

        // Clamp speed
        _rb.linearVelocity = Vector3.ClampMagnitude(_rb.linearVelocity, _maxSpeed);

        // --- 2. Steering (only when moving) ---
        if (Mathf.Abs(_currentSpeed) > 0.5f)
        {
            float speedFactor = Mathf.Clamp01(Mathf.Abs(_currentSpeed) / _maxSpeed);
            float driftMultiplier = isDrifting ? _driftTurnBoost : 1f;

            float turnAmount = turnInput * _rotationSpeed * speedFactor * driftMultiplier * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
            _rb.MoveRotation(_rb.rotation * turnRotation);
        }

        // --- 3. Drift Traction ---
        Vector3 localVel = transform.InverseTransformDirection(_rb.linearVelocity);
        float targetTraction = isDrifting ? _driftTraction : _traction;
        localVel.x = Mathf.Lerp(localVel.x, 0, Time.deltaTime * targetTraction);
        _rb.linearVelocity = transform.TransformDirection(localVel);

        // --- 4. Downforce (for stability) ---
        _rb.AddForce(-transform.up * _downforce * _rb.linearVelocity.magnitude);

        // --- 5. Visual Drift Tilt ---
        if (isDrifting && Mathf.Abs(turnInput) > 0.1f)
        {
            float tilt = Mathf.Lerp(0, 15f, Mathf.Abs(turnInput));
            Quaternion tiltRot = Quaternion.Euler(0, 0, -tilt * Mathf.Sign(turnInput));
            transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * tiltRot, Time.deltaTime * 0.5f);
        }
    }

    private bool GroundCheck()
    {
        return Physics.Raycast(transform.position + transform.up * 0.5f, -transform.up, _groundRayLength, LayerMask.GetMask("Floor"));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 start = transform.position + transform.up * 0.5f;
        Vector3 end = start + (-transform.up * _groundRayLength);
        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.05f);
    }
}
