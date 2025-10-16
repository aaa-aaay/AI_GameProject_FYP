using UnityEngine;

public class BetterCarMovement : MonoBehaviour
{
    [Header("References")]
    public Transform kartModel;
    public Transform kartNormal;
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

    private void Update()
    {
        // Currently empty — could be used for camera or VFX updates later
    }

    public void MoveCar(Vector2 inputDir, bool isDrifting)
    {
        // Position car body with the physics sphere
        transform.position = sphere.transform.position - new Vector3(0, 1.0f, 0);

        float forwardInput = inputDir.y;
        float turnInput = inputDir.x;

        // Forward acceleration
        if (forwardInput > 0.1f)
        {
            speed = acceleration;
        }

        // Reverse (disabled for now)
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

        // Smooth acceleration
        currentSpeed = Mathf.SmoothStep(currentSpeed, speed, Time.deltaTime * 12f);
        speed = 0f;

        // Smooth rotation
        currentRotate = Mathf.Lerp(currentRotate, rotate, Time.deltaTime * 4f);
        rotate = 0f;

        // Adjust model tilt if not drifting
        if (!isDrifting)
        {
            kartModel.localRotation = Quaternion.Lerp(
                kartModel.localRotation,
                Quaternion.Euler(0f, turnInput * 15f, 0f),
                0.2f
            );
        }
    }

    private void FixedUpdate()
    {
        // Apply forward and downward forces
        sphere.AddForce(transform.forward * currentSpeed, ForceMode.Acceleration);
        sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);

        // Rotate the car properly based on steering input
        transform.Rotate(0, currentRotate * Time.fixedDeltaTime, 0, Space.World);

        // Raycasts for terrain alignment
        Physics.Raycast(transform.position + (transform.up * 0.1f), Vector3.down, out RaycastHit hitOn, 1.5f, layerMask);
        Physics.Raycast(transform.position + (transform.up * 0.1f), Vector3.down, out RaycastHit hitNear, 2.0f, layerMask);

        // Align car normal to terrain
        kartNormal.up = Vector3.Lerp(kartNormal.up, hitNear.normal, Time.deltaTime * 8.0f);
        kartNormal.Rotate(0, transform.eulerAngles.y, 0);
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
