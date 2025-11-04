using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TaggerAgent : Agent
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private MeshRenderer plane;
    [SerializeField] private Material success;
    [SerializeField] private Material fail;
    [SerializeField] private TimerUI timerUI;
    
    public enum AgentMode { Training, Inference }

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float jumpForce = 6f;
    public float approachReward = 0.01f;
    public float sameHeightReward = 0.01f;
    public float gravity = 2f;
    [Header("Agent Mode")]
    public AgentMode agentMode = AgentMode.Training;

    private Rigidbody rb;
    private bool isGrounded = true;
    private Vector3 lastPosition;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        lastPosition = transform.position;

        if (target == null)
        {
            PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
            if (player != null)
                target = player.transform;
        }
    }
    private void FixedUpdate()
    {
        // Apply extra gravity when not grounded
        if (!Physics.Raycast(transform.position, Vector3.down, 0.6f))
        {
            rb.AddForce(Physics.gravity * gravity, ForceMode.Acceleration);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (agentMode != AgentMode.Training) return;

        // Removed wall penalty

        if (collision.collider.CompareTag("Border"))
        {
            AddReward(-10f);
            if (plane != null && fail != null) plane.material = fail;
            EndEpisode();
        }

        if (collision.collider.CompareTag("Runner"))
        {
            AddReward(5f);
            if (plane != null && success != null) plane.material = success;
            EndEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (agentMode == AgentMode.Training)
        {
            transform.localPosition = new Vector3(Random.Range(-21, 21), 0.5f, Random.Range(-21, 21));
            if (target != null)
                target.localPosition = new Vector3(Random.Range(-21, 21), 0.5f, Random.Range(-21, 21));

            if (timerUI != null)
            {
                timerUI.ResetTimer();
                timerUI.StartTimer();
            }
        }

        lastPosition = transform.position;
        isGrounded = true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(isGrounded ? 1f : 0f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveForward = actions.ContinuousActions[0];
        float moveRight = actions.ContinuousActions[1];
        float jumpInput = actions.ContinuousActions[2];

        // Rotation toward target (only visual)
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.fixedDeltaTime * 10f);
        }

        // Movement input
        Vector3 moveDir = (transform.forward * moveForward + transform.right * moveRight).normalized;
        Vector3 move = moveDir * moveSpeed;

        if (agentMode == AgentMode.Inference)
        {
            // ✅ Use velocity-based movement for real physics collisions
            rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        }
        else
        {
            // ✅ Use MovePosition only during training for smoother results
            rb.MovePosition(transform.position + move * Time.fixedDeltaTime);
        }

        // Jump
        if (jumpInput > 0.5f && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        // Apply custom gravity if not grounded
        if (!isGrounded)
        {
            rb.AddForce(Physics.gravity * gravity, ForceMode.Acceleration);
        }

        // ===== REWARDS (Training Only) =====
        if (agentMode == AgentMode.Training)
        {
            float prevDistance = Vector3.Distance(lastPosition, target.position);
            float newDistance = Vector3.Distance(transform.position, target.position);
            AddReward(approachReward * (prevDistance - newDistance));

            float heightDiff = Mathf.Abs(transform.position.y - target.position.y);
            if (heightDiff < 0.5f)
                AddReward(sameHeightReward);

            if (timerUI != null && timerUI.GetRemainingTime() <= 0f)
            {
                AddReward(-1f);
                if (plane != null && fail != null) plane.material = fail;
                EndEpisode();
            }
        }

        lastPosition = transform.position;
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuous = actionsOut.ContinuousActions;
        continuous[0] = Input.GetAxisRaw("Vertical");
        continuous[1] = Input.GetAxisRaw("Horizontal");
        continuous[2] = Input.GetKey(KeyCode.Space) ? 1f : 0f; // Jump
    }
}
