using UnityEngine;

public class RaceCarMovement : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _maxSpeed = 20;
    [SerializeField] private float _acce = 1.0f;
    [SerializeField] private float _rotationPerSecond = 1.0f;


    [SerializeField] private float _groundRayLength = 2.0f;
    private float _currentSpeed;


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void MoveCar(Vector2 inputDir, bool isDrift)
    {


        float forwardInput = inputDir.y;
        float sidewaysInput = inputDir.x;

        if (!GroundCheck()) return;

        //********Rotation feature******//
        var rotationAmtInCurrentFrame = _rotationPerSecond * Time.deltaTime;
        rotationAmtInCurrentFrame *= _currentSpeed / _maxSpeed;
        if (isDrift) rotationAmtInCurrentFrame /= 1.5f;

        if (sidewaysInput > 0.1) transform.rotation = Quaternion.RotateTowards(transform.rotation,
            Quaternion.LookRotation(transform.right), rotationAmtInCurrentFrame);
        else if(sidewaysInput < -0.1) transform.rotation = Quaternion.RotateTowards(transform.rotation,
            Quaternion.LookRotation(-transform.right), rotationAmtInCurrentFrame);


        //*******Move Front Back********//

        //if either forward or back inputs are pressed, accelerate (+deltaSpeed) or (-deltaSpeed)
        var deltaSpeed = _acce * Time.deltaTime;

        if (forwardInput > 0.1) _currentSpeed += deltaSpeed;
        else if (forwardInput < -0.1) _currentSpeed -= deltaSpeed;

        else if (_currentSpeed > 0) _currentSpeed = Mathf.Max(_currentSpeed - deltaSpeed, 0);
        else if (_currentSpeed < 0) _currentSpeed = Mathf.Min(_currentSpeed + deltaSpeed, 0);

        var displacementDirection = transform.forward;


        //drfiting
        if (isDrift)
        {
            if (sidewaysInput < 0.1)
                displacementDirection += transform.right;
            if (sidewaysInput > 0.1)
                displacementDirection -= transform.right;
        }


        _currentSpeed = Mathf.Clamp(_currentSpeed, -_maxSpeed, _maxSpeed);
        var desiredVelocity = displacementDirection.normalized * _currentSpeed;

        desiredVelocity.y = _rb.linearVelocity.y;

        _rb.linearVelocity = desiredVelocity;


        float driftFactor = isDrift ? 0.6f : 0.3f;
        ApplyLateralFriction(driftFactor);
        ApplyDownforce();


        if (isDrift)
        {
            float driftAngle = Mathf.Lerp(0, 2f, Mathf.Abs(sidewaysInput));
            Quaternion driftRotation = Quaternion.Euler(0, driftAngle * Mathf.Sign(sidewaysInput), 0);
            transform.rotation *= driftRotation;
        }

    }


    private void ApplyLateralFriction(float driftFactor)
    {
        Vector3 forwardVelocity = Vector3.Project(_rb.linearVelocity, transform.forward);
        Vector3 rightVelocity = Vector3.Project(_rb.linearVelocity, transform.right);

        _rb.linearVelocity = forwardVelocity + rightVelocity * driftFactor;
    }

    private void ApplyDownforce()
    {
        _rb.AddForce(-transform.up * _rb.linearVelocity.magnitude * 0.5f, ForceMode.Acceleration);
    }

    private bool GroundCheck()
    {
        return Physics.Raycast(transform.position + transform.up, -transform.up, _groundRayLength, LayerMask.GetMask("Floor"));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 start = transform.position + transform.up;
        Vector3 end = start + (-transform.up * _groundRayLength);
        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.05f);
    }


}
