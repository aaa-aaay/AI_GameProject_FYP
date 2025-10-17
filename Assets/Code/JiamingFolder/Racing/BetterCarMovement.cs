using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class BetterCarMovement : MonoBehaviour
{
    [Header("References")]
    public Transform kartModel;
    public Transform kartNormal;
    public Transform _rightWheel;
    public Transform _rightWheelPivot;
    public Transform _leftWheel;
    public Transform _leftWheelPivot;
    public Transform _backWheel;
    public Rigidbody sphere;

    [Header("Parameters")]
    public float acceleration = 30f;
    public float steering = 80f;
    public float gravity = 10f;
    public LayerMask layerMask;

    private float speed;
    private float currentSpeed;
    private float rotate;
    private float currentRotate;


    private bool drifting;


    int driftDirection;
    float driftPower;
    int driftMode = 0;
    bool first, second, third;

    private void Update()
    {
        // Currently empty — could be used for camera or VFX updates later
    }

    public void MoveCar(Vector2 inputDir)
    {
        // Position car body with the physics sphere
        transform.position = sphere.transform.position - new Vector3(0, 1.0f, 0);


        float forwardInput = inputDir.y;
        float turnInput = inputDir.x;

        driftDirection = turnInput > 0? 1 : -1;

        // Forward acceleration
        if (forwardInput > 0.1f)
        {
            speed = acceleration;
        }

        if (forwardInput < -0.1f)
        {
            speed = -acceleration;
        }

        // Steering
        if (Mathf.Abs(turnInput) > 0.1f)
        {
            int dir = turnInput > 0 ? 1 : -1;
            float amount = Mathf.Abs(turnInput);
            Steer(dir, amount);
        }



        if (drifting)
        {
            float control = (driftDirection == 1) ? MyMathUtils.Remap(turnInput, -1, 1, 0, 2) : MyMathUtils.Remap(turnInput, -1, 1, 2, 0);
            float powerControl = (driftDirection == 1) ? MyMathUtils.Remap(turnInput, -1, 1, .2f, 1) : MyMathUtils.Remap(turnInput, -1, 1, 1, .2f);
            Steer(driftDirection, control);
            driftPower += powerControl;

            //ColorDrift();

        }



        // Smooth acceleration
        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f);
        speed = 0f;

        // Smooth rotation
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f);
        rotate = 0f;

        // Adjust model tilt if not drifting
        if (!drifting)
        {
            kartModel.localRotation = Quaternion.Lerp(
                kartModel.localRotation,
                Quaternion.Euler(0f, turnInput * 15f, 0f),
                0.2f
            );
        }
        else
        {
            kartModel.localRotation = Quaternion.Lerp(
            kartModel.localRotation,
            Quaternion.Euler(0f, 10f * driftDirection, 0f ), // roll tilt
            0.1f );
        }

        // Calculate steering (applied to pivot)
        Quaternion steerRot = Quaternion.Euler(0, turnInput * steering * 3, 0);
        _rightWheelPivot.localRotation = steerRot;
        _leftWheelPivot.localRotation = steerRot;

        // Calculate rolling
        float localZVel = transform.InverseTransformDirection(sphere.linearVelocity).z;
        float wheelRadius = 0.35f;
        float rotationPerMeter = 360f / (2 * Mathf.PI * wheelRadius);
        float rotationAmount = localZVel * rotationPerMeter * Time.deltaTime;

        // Apply spin to mesh (not pivot)
        _rightWheel.Rotate(Vector3.right, rotationAmount, Space.Self);
        _leftWheel.Rotate(Vector3.right, rotationAmount, Space.Self);
        _backWheel.Rotate(Vector3.right, rotationAmount, Space.Self);


    }

    private void FixedUpdate()
    {
        // Apply forward and downward forces
        if (drifting)
            sphere.AddForce(-kartModel.transform.right * currentSpeed, ForceMode.Acceleration);
        else
            sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);

        //sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);
        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        // Rotate the car properly based on steering input
        transform.eulerAngles = Vector3.Lerp(
            transform.eulerAngles,
            new Vector3(0, transform.eulerAngles.y + currentRotate, 0),
            Time.deltaTime * 5f
        );

        // Raycasts for terrain alignment
        Physics.Raycast(transform.position + (transform.up * 0.1f), Vector3.down, out RaycastHit hitOn, 1.5f, layerMask);
        Physics.Raycast(transform.position + (transform.up * 0.1f), Vector3.down, out RaycastHit hitNear, 2.0f, layerMask);

        // Align car normal to terrain
        kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        kartNormal.Rotate(0, transform.eulerAngles.y, 0);
    }

    public void ToggleDrifting(bool startDrift, float turnInput = 0)
    {


        if (startDrift && !drifting)
        {
            drifting = true;
            driftDirection = turnInput > 0 ? 1 : -1;
            kartModel.parent.DOComplete();
            kartModel.parent.DOPunchPosition(transform.up * .9f, .3f, 5, 1);
        }

        if (startDrift == false && drifting) {
            drifting = false;
            kartModel.parent.DOLocalRotate(Vector3.zero, .5f).SetEase(Ease.OutBack);

        }

    }
    public void Steer(int direction, float amount)
    {
        rotate = steering * direction * amount;
    }

    private void Speed(float x)
    {
        currentSpeed = x;
    }


}
