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
    public float tagReward = 5f;
    public float losePlayerPenalty = -1f;
    public float edgePenalty = -10f;
    public float crowdPenalty = -0.5f;
    public float staminaPenalty = -0.2f;
    public float exploreReward = 0.5f; // reward for reaching exploration point

    [Header("Awareness Settings")]
    public float detectionRadius = 10f;

    [Header("Stamina Settings")]
    public float maxStamina = 5f;
    public float staminaRegenRate = 1f;
    public float staminaDrainRate = 1.5f;
    public float movementThreshold = 0.05f;

    [SerializeField] private List<TaggerAgent> otherTaggers = new List<TaggerAgent>();

    private Rigidbody rb;
    private bool isGrounded = true;
    private float currentStamina;
    private bool playerInRange;

    // --- Exploration Point ---
    private Vector3 explorePoint;
    private float exploreThreshold = 2f;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        currentStamina = maxStamina;

        if (target == null)
        {
            GameObject runnerObj = GameObject.FindWithTag("Runner");
            if (runnerObj != null)
                target = runnerObj.transform;
        }

        if (otherTaggers.Count == 0)
            FindOtherTaggers();

        GenerateNewExplorePoint();
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

    public override void OnEpisodeBegin()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (agentMode == AgentMode.Training)
        {
            transform.localPosition = new Vector3(Random.Range(-18f, 18f), 0f, Random.Range(-18f, 18f));

            if (target != null)
                target.localPosition = new Vector3(Random.Range(-18f, 18f), 0f, Random.Range(-18f, 18f));

            if (timerUI != null)
            {
                timerUI.ResetTimer();
                timerUI.StartTimer();
            }
        }

        if (otherTaggers.Count == 0)
            FindOtherTaggers();

        currentStamina = maxStamina;
        playerInRange = false;
        isGrounded = true;
        GenerateNewExplorePoint();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target != null ? target.localPosition : Vector3.zero);
        sensor.AddObservation(isGrounded ? 1f : 0f);
        sensor.AddObservation(currentStamina / maxStamina);
        sensor.AddObservation(playerInRange ? 1f : 0f);

        // Exploration point observation
        sensor.AddObservation(explorePoint);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (target == null) return;

        float moveForward = actions.ContinuousActions[0];
        float moveRight = actions.ContinuousActions[1];
        float jumpInput = actions.ContinuousActions[2];

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

        Vector3 moveDir;

        // Determine movement direction
        if (playerInRange)
        {
            // Chase player
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0f;
            moveDir = direction;
        }
        else
        {
            // Explore when no player nearby
            Vector3 direction = (explorePoint - transform.position).normalized;
            direction.y = 0f;
            moveDir = direction;

            float distToExplore = Vector3.Distance(transform.position, explorePoint);
            if (distToExplore < exploreThreshold)
            {
                AddReward(exploreReward);
                GenerateNewExplorePoint();
            }
        }

        Vector3 move = moveDir * moveSpeed * Mathf.Clamp01(moveForward);
        bool isMoving = moveDir.magnitude > movementThreshold;

        // --- Rotation towards runner or explore point ---
        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.identity;
            if (playerInRange)
            {
                Vector3 directionToRunner = (target.position - transform.position).normalized;
                directionToRunner.y = 0f;
                targetRotation = Quaternion.LookRotation(directionToRunner);
            }
            else
            {
                Vector3 directionToExplore = (explorePoint - transform.position).normalized;
                directionToExplore.y = 0f;
                targetRotation = Quaternion.LookRotation(directionToExplore);
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5f);
        }

        // Stamina
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

        // Movement application
        if (agentMode == AgentMode.Inference)
            rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        else
            rb.MovePosition(transform.position + move * Time.fixedDeltaTime);

        // --- AUTO JUMP LOGIC (only in Inference mode) ---
        if (agentMode == AgentMode.Inference)
        {
            // Jump if blocked or if player is higher than us
            bool blocked = Physics.Raycast(transform.position, transform.forward, 1f);
            bool playerAbove = (target.position.y - transform.position.y) > 1.0f;

            if ((blocked || playerAbove) && isGrounded && currentStamina > 0.5f)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;
                currentStamina -= 0.5f;
            }
        }
        else if (jumpInput > 0.5f && isGrounded && currentStamina > 0.5f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            currentStamina -= 0.5f;
        }

        if (!isGrounded)
            rb.AddForce(Physics.gravity * gravity, ForceMode.Acceleration);

        // Crowd penalty
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
    }


    private void GenerateNewExplorePoint()
    {
        explorePoint = new Vector3(Random.Range(-20f, 20f), 0f, Random.Range(-20f, 20f));
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuous = actionsOut.ContinuousActions;
        continuous[0] = Input.GetAxisRaw("Vertical");
        continuous[1] = Input.GetAxisRaw("Horizontal");
        continuous[2] = Input.GetKey(KeyCode.Space) ? 1f : 0f;
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
        if (collision.collider.CompareTag("Border"))
        {
            AddReward(edgePenalty);
            plane.material = fail;
            EndEpisodeForAll();
        }
        else if (collision.collider.CompareTag("Runner"))
        {
            AddReward(tagReward);
            plane.material = success;
            EndEpisodeForAll();
        }
    }


    private void EndEpisodeForAll()
    {
        EndEpisode();
        foreach (var tagger in otherTaggers)
        {
            if (tagger != null && tagger.enabled)
                tagger.EndEpisode();
        }

        if (target != null)
        {
            var runnerAgent = target.GetComponent<RunnerAgent>();
            if (runnerAgent != null)
                runnerAgent.EndEpisode();
        }
    }
}
