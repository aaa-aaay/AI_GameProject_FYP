using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class RunnerAgent : Agent
{
    [Header("References")]
    public Transform tagger;          
    public TimerUI timerUI;

    [Header("Movement Settings")]
    public float moveSpeed = 2.5f;
    public float jumpForce = 6f;
    public float avoidReward = 0.02f;      
    public float heightReward = 0.005f;     
    public float gravity = 2f;
    [Header("Jump Settings")]
    public float jumpChance = 0.0025f;    
    private bool isGrounded = true;
    private bool jumpedRecently = false;
    private bool caughtAfterJump = false;

    private Rigidbody rb;
    private Vector3 lastPosition;
    private float lastDistance;
    private Coroutine jumpPenaltyCheck;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (tagger == null)
        {
            TaggerAgent tag = FindFirstObjectByType<TaggerAgent>();
            if (tag != null) tagger = tag.transform;
        }

        lastPosition = transform.position;
    }
    private void FixedUpdate()
    {
    
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

        if (collision.collider.CompareTag("Border"))
        {
            AddReward(-10f);
            EndEpisode();
        }

        if (collision.collider.CompareTag("Tagger"))
        {
            // Runner caught
            AddReward(-5f);

            if (jumpedRecently)
            {
                caughtAfterJump = true;
                AddReward(-10f); // Heavier penalty for being caught after a jump
            }

           
            EndEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.localPosition = new Vector3(Random.Range(-21, 21), 0.5f, Random.Range(-21, 21));

        if (tagger != null)
            tagger.localPosition = new Vector3(Random.Range(-21, 21), 0.5f, Random.Range(-21, 21));

        if (timerUI != null)
        {
            timerUI.ResetTimer();
            timerUI.StartTimer();
        }

        lastDistance = Vector3.Distance(transform.position, tagger.position);
        isGrounded = true;
        jumpedRecently = false;
        caughtAfterJump = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition); // Runner position
        sensor.AddObservation(tagger.localPosition);    // Tagger position
        sensor.AddObservation(isGrounded ? 1f : 0f);    // Is grounded
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveForward = actions.ContinuousActions[0];
        float moveRight = actions.ContinuousActions[1];
        float jumpInput = actions.ContinuousActions[2]; // Jump input

        Vector3 move = (transform.forward * moveForward + transform.right * moveRight) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + move);

        // Random jump trigger during inference
        if (!jumpedRecently && isGrounded && Random.value < jumpChance)
        {
            Jump();
        }

        // ML can also choose to jump
        if (jumpInput > 0.5f && isGrounded)
        {
            Jump();
        }

        // Rewards
        float newDistance = Vector3.Distance(transform.position, tagger.position);
        float distanceDelta = newDistance - lastDistance;

        // Reward for increasing distance
        AddReward(avoidReward * distanceDelta);

        // Reward for being on a different Y level
        float heightDiff = Mathf.Abs(transform.position.y - tagger.position.y);
        AddReward(heightDiff * heightReward);

        lastDistance = newDistance;

        // Episode ends if timer runs out
        if (timerUI != null && timerUI.GetRemainingTime() <= 0f)
        {
            AddReward(2f); // Survived till end
          
            EndEpisode();
        }
    }

    private void Jump()
    {
        if (!isGrounded) return;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
        jumpedRecently = true;

        // Start jump penalty check coroutine
        if (jumpPenaltyCheck != null) StopCoroutine(jumpPenaltyCheck);
        jumpPenaltyCheck = StartCoroutine(JumpPenaltyWindow());
    }

    private IEnumerator JumpPenaltyWindow()
    {
        yield return new WaitForSeconds(5f);
        jumpedRecently = false;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuous = actionsOut.ContinuousActions;
        continuous[0] = Input.GetAxisRaw("Vertical");
        continuous[1] = Input.GetAxisRaw("Horizontal");
        continuous[2] = Input.GetKey(KeyCode.Space) ? 1f : 0f;
    }
}
