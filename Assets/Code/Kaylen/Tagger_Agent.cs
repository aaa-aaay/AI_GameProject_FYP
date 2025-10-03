using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using static Runner;

[RequireComponent(typeof(Rigidbody))]
public class Tagger : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private MeshRenderer plane;
    [SerializeField] private Material success;
    [SerializeField] private Material fail;
    [SerializeField] private TimerUI timerUI;
    public enum AgentMode { Training, Inference }
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float approachReward = 0.01f;

    [Header("Agent Mode")]
    public AgentMode agentMode = AgentMode.Training; // Toggle in Inspector

    private Rigidbody rb;
    private Vector3 lastPosition;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        lastPosition = transform.position;

        if (target == null)
        {
            PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
            if (player != null) target = player.transform;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (agentMode != AgentMode.Training) return;

        if (collision.collider.CompareTag("Wall"))
        {
            AddReward(-5f);
            if (plane != null && fail != null) plane.material = fail;
            EndEpisode();
        }

        if (collision.collider.CompareTag("Border"))
        {
            AddReward(-20f);
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
            {
                target.localPosition = new Vector3(Random.Range(-21, 21), 0.5f, Random.Range(-21, 21));
            }

            if (timerUI != null)
            {
                timerUI.ResetTimer();
                timerUI.StartTimer();
            }
        }

        lastPosition = transform.position;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveForward = actions.ContinuousActions[0];
        float moveRight = actions.ContinuousActions[1];

        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.fixedDeltaTime * 10f);
        }

        Vector3 move = (transform.forward * moveForward + transform.right * moveRight) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + move);

        if (agentMode == AgentMode.Training)
        {
            float prevDistance = Vector3.Distance(lastPosition, target.position);
            float newDistance = Vector3.Distance(transform.position, target.position);
            AddReward(approachReward * (prevDistance - newDistance));

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
    }
}
