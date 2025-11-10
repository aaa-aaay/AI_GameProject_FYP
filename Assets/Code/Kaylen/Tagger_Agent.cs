using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using System.Collections.Generic;

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
    public AgentMode agentMode = AgentMode.Training;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float jumpForce = 6f;
    public float gravity = 2f;

    [Header("Reward Settings")]
    public float approachReward = 0.01f;
    public float sameHeightReward = 0.01f;
    public float tagReward = 5f;
    public float losePlayerPenalty = -1f;
    public float edgePenalty = -10f;
    public float crowdPenalty = -0.5f;
    public float staminaPenalty = -0.2f;

    [Header("Awareness Settings")]
    public float detectionRadius = 10f;

    [Header("Stamina Settings")]
    public float maxStamina = 5f;
    public float staminaRegenRate = 1f;
    public float staminaDrainRate = 1.5f;
    public float movementThreshold = 0.05f;

    [Header("Other Taggers (Assign Manually if Desired)")]
    [SerializeField] private List<TaggerAgent> otherTaggers = new List<TaggerAgent>();

    private Rigidbody rb;
    private bool isGrounded = true;
    private Vector3 lastPosition;
    private float currentStamina;
    private bool playerInRange;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        lastPosition = transform.position;
        currentStamina = maxStamina;

        if (target == null)
        {
            GameObject runnerObj = GameObject.FindWithTag("Runner");
            if (runnerObj != null)
                target = runnerObj.transform;
        }

        if (otherTaggers.Count == 0)
            FindOtherTaggers();
    }

    private void FindOtherTaggers()
    {
        otherTaggers.Clear();
        GameObject[] taggerObjs = GameObject.FindGameObjectsWithTag("Tagger");
        foreach (var obj in taggerObjs)
        {
            TaggerAgent tagger = obj.GetComponent<TaggerAgent>();
            if (tagger != null && tagger != this)
                otherTaggers.Add(tagger);
        }
    }

    private void FixedUpdate()
    {
        if (!Physics.Raycast(transform.position, Vector3.down, 0.6f))
        {
            rb.AddForce(Physics.gravity * gravity, ForceMode.Acceleration);
            isGrounded = false;
        }
        else
        {
            isGrounded = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
            isGrounded = true;

        if (agentMode != AgentMode.Training) return;

        // --- Border collision ---
        if (collision.collider.CompareTag("Border"))
        {
            AddReward(edgePenalty);
            if (plane != null && fail != null) plane.material = fail;

            // NEW: synchronize episode end for all taggers
            EndEpisodeForAll();
        }

        // --- Runner collision ---
        if (collision.collider.CompareTag("Runner"))
        {
            AddReward(tagReward);
            if (plane != null && success != null) plane.material = success;

            // NEW: synchronize episode end for all taggers
            EndEpisodeForAll();
        }
    }

    public override void OnEpisodeBegin()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (agentMode == AgentMode.Training)
        {
            transform.localPosition = new Vector3(Random.Range(-18f, 18f), 1.2f, Random.Range(-18f, 18f));

            if (target == null)
            {
                GameObject runnerObj = GameObject.FindWithTag("Runner");
                if (runnerObj != null)
                    target = runnerObj.transform;
            }

            if (target != null)
                target.localPosition = new Vector3(Random.Range(-18f, 18f), 0.5f, Random.Range(-18f, 18f));

            if (timerUI != null)
            {
                timerUI.ResetTimer();
                timerUI.StartTimer();
            }
        }

        if (otherTaggers.Count == 0)
            FindOtherTaggers();

        lastPosition = transform.position;
        currentStamina = maxStamina;
        playerInRange = false;
        isGrounded = true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target != null ? target.localPosition : Vector3.zero);
        sensor.AddObservation(isGrounded ? 1f : 0f);
        sensor.AddObservation(currentStamina / maxStamina);
        sensor.AddObservation(playerInRange ? 1f : 0f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (target == null) return;

        float moveForward = actions.ContinuousActions[0];
        float moveRight = actions.ContinuousActions[1];
        float jumpInput = actions.ContinuousActions[2];

        // Detection logic
        float playerDistance = Vector3.Distance(transform.position, target.position);
        bool playerCurrentlyInRange = playerDistance <= detectionRadius;

        if (playerCurrentlyInRange)
        {
            AddReward(approachReward);
            playerInRange = true;
        }
        else
        {
            if (playerInRange)
                AddReward(losePlayerPenalty);
            playerInRange = false;
        }

        if (playerInRange)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion lookRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.fixedDeltaTime * 10f);
            }
        }

        Vector3 moveDir = (transform.forward * moveForward + transform.right * moveRight).normalized;
        Vector3 move = moveDir * moveSpeed;
        bool isMoving = moveDir.magnitude > movementThreshold;

        // Stamina system
        if (isMoving)
        {
            currentStamina -= staminaDrainRate * Time.fixedDeltaTime;
            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                AddReward(staminaPenalty);
            }
        }
        else
        {
            currentStamina = Mathf.Min(maxStamina, currentStamina + staminaRegenRate * Time.fixedDeltaTime);
        }

        if (agentMode == AgentMode.Inference)
            rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        else
            rb.MovePosition(transform.position + move * Time.fixedDeltaTime);

        if (jumpInput > 0.5f && isGrounded && currentStamina > 0.5f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            currentStamina -= 0.5f;
        }

        if (!isGrounded)
            rb.AddForce(Physics.gravity * gravity, ForceMode.Acceleration);

        // Penalize clustering
        foreach (var other in otherTaggers)
        {
            if (other == null) continue;
            float dist = Vector3.Distance(transform.position, other.transform.position);
            if (dist < detectionRadius)
                AddReward(crowdPenalty);
        }

        // Timer failure
        if (agentMode == AgentMode.Training && timerUI != null && timerUI.GetRemainingTime() <= 0f)
        {
            AddReward(-1f);
            if (plane != null && fail != null) plane.material = fail;
            EndEpisodeForAll();
        }

        lastPosition = transform.position;
    }

    // --- NEW METHOD: Synchronized episode ending ---
    private void EndEpisodeForAll()
    {
        EndEpisode(); // end self
        foreach (var tagger in otherTaggers)
        {
            if (tagger != null && tagger.enabled)
                tagger.EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuous = actionsOut.ContinuousActions;
        continuous[0] = Input.GetAxisRaw("Vertical");
        continuous[1] = Input.GetAxisRaw("Horizontal");
        continuous[2] = Input.GetKey(KeyCode.Space) ? 1f : 0f;
    }
}
