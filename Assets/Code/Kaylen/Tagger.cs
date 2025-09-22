using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Tagger : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private MeshRenderer plane;
    [SerializeField] private Material success;
    [SerializeField] private Material fail;
    [SerializeField] private TimerUI timerUI;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float approachReward = 0.01f;

    private Rigidbody rb;
    private Vector3 lastPosition;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        lastPosition = transform.position;

        // Auto-assign target if not set in Inspector
        if (target == null)
        {
            PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
            if (player != null)
            {
                target = player.transform;
                Debug.Log($"Tagger {name} locked onto {player.name}");
            }
            else
            {
                Debug.LogWarning("Tagger could not find PlayerMovement in the scene.");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            AddReward(-5f);   // big penalty for touching wall
            if (plane != null && fail != null) plane.material = fail;
            EndEpisode();
        }

        if (collision.collider.CompareTag("Runner"))
        {
            AddReward(5f); // huge reward for tagging
            if (plane != null && success != null) plane.material = success;
            EndEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Spawn in large arena
        //transform.localPosition = new Vector3(Random.Range(-21, 21), 0.5f, Random.Range(-21, 21));
        //if (target != null)
        //{
        //    target.localPosition = new Vector3(Random.Range(-21, 21), 0.5f, Random.Range(-21, 21));
        //}

        if (timerUI != null)
        {
            timerUI.ResetTimer();
            timerUI.StartTimer();
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

        // Always face the target
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.fixedDeltaTime * 10f);
        }

        // Movement
        Vector3 move = (transform.forward * moveForward + transform.right * moveRight) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + move);

        // Reward for approaching the runner
        float prevDistance = Vector3.Distance(lastPosition, target.position);
        float newDistance = Vector3.Distance(transform.position, target.position);
        AddReward(approachReward * (prevDistance - newDistance));

        lastPosition = transform.position;

        // Timeout punishment
        if (timerUI != null && timerUI.GetRemainingTime() <= 0f)
        {
            AddReward(-1f);
            if (plane != null && fail != null) plane.material = fail;
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuous = actionsOut.ContinuousActions;
        continuous[0] = Input.GetAxisRaw("Vertical");   // forward/back
        continuous[1] = Input.GetAxisRaw("Horizontal"); // strafe left/right
    }
}
