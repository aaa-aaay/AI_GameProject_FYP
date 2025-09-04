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
    public float moveSpeed = 1f;    
    public float jumpForce = 5f;



    private Rigidbody rb;
    private bool isGrounded = true;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // prevent tumbling
    }

    public override void OnEpisodeBegin()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.localPosition = new Vector3(Random.Range(-10, 10), 0.5f, Random.Range(-10 , 10));
        target.localPosition = new Vector3(Random.Range(-10, 10), 0.5f, Random.Range(-10, 10));

        // Reset the timer at the start of each episode
        if (timerUI != null)
        {
            timerUI.ResetTimer();
            timerUI.StartTimer();
        }
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(isGrounded ? 1 : 0);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        float jump = actions.ContinuousActions[2];

        // Movement
        Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + move);

        // Jump
        if (jump > 0.5f && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        // Countdown
        if (timerUI != null && timerUI.GetRemainingTime() <= 0f)
        {
            AddReward(-1f); // punish for not catching runner in time
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        else if (collision.collider.CompareTag("Runner"))
        {
            AddReward(1f);
            if (plane != null && success != null) plane.material = success;
            EndEpisode();
        }
        else if (collision.collider.CompareTag("Wall"))
        {
            AddReward(-1f);
            if (plane != null && fail != null) plane.material = fail;
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuous = actionsOut.ContinuousActions;
        continuous[0] = Input.GetAxisRaw("Horizontal");   // X movement
        continuous[1] = Input.GetAxisRaw("Vertical");     // Z movement
        continuous[2] = Input.GetButton("Jump") ? 1f : 0f; // Jump action
    }
}
