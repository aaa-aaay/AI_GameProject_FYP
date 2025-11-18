using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RunnerAgent : Agent
{
    [Header("References")]
    public Transform safePoint;
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public Vector3 arenaBounds = new Vector3(20f, 0f, 20f);
    public float detectionRadius = 5f;
    [SerializeField] private TaggerAgent[] taggers;

    private Rigidbody rb;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        if (taggers == null || taggers.Length == 0)
            taggers = FindObjectsOfType<TaggerAgent>();
    }

    public override void OnEpisodeBegin()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Random runner position
        transform.localPosition = new Vector3(
            Random.Range(-arenaBounds.x, arenaBounds.x),
            0.5f,
            Random.Range(-arenaBounds.z, arenaBounds.z)
        );

        // Random safe point
        if (safePoint != null)
        {
            safePoint.localPosition = new Vector3(
                Random.Range(-arenaBounds.x, arenaBounds.x),
                0.5f,
                Random.Range(-arenaBounds.z, arenaBounds.z)
            );
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(safePoint != null ? safePoint.localPosition : Vector3.zero);
        sensor.AddObservation(rb.linearVelocity);

        foreach (var tagger in taggers)
        {
            sensor.AddObservation(tagger != null ? tagger.transform.localPosition : Vector3.zero);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveZ = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        Vector3 moveDir = new Vector3(moveX, 0f, moveZ);
        rb.linearVelocity = new Vector3(moveDir.normalized.x * moveSpeed, rb.linearVelocity.y, moveDir.normalized.z * moveSpeed);

        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.fixedDeltaTime * rotationSpeed);
        }

        // Small reward for moving closer to safe point
        if (safePoint != null)
        {
            float distToSafe = Vector3.Distance(transform.position, safePoint.position);
            AddReward(0.001f * (1f / distToSafe));
        }

        // Penalize if near taggers
        foreach (var tagger in taggers)
        {
            if (tagger != null && Vector3.Distance(transform.position, tagger.transform.position) < detectionRadius)
            {
                AddReward(-0.01f);
            }
        }

        // Penalize if outside arena bounds
        if (Mathf.Abs(transform.localPosition.x) > arenaBounds.x || Mathf.Abs(transform.localPosition.z) > arenaBounds.z)
        {
            AddReward(-0.1f);
            EndEpisodeForAll();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == safePoint)
        {
            AddReward(1f); 

            safePoint.position = new Vector3(
                Random.Range(-20f, 20f),
                safePoint.position.y,
                Random.Range(-20f, 20f)
            );
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Tagger"))
        {
            AddReward(-1f);
            EndEpisodeForAll();
        }
        else if (collision.collider.CompareTag("Border"))
        {
            AddReward(-0.5f);
            EndEpisodeForAll();
        }
    }
    private void EndEpisodeForAll()
    {
        EndEpisode();
        foreach (var tagger in taggers)
        {
            if (tagger != null)
                tagger.EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuous = actionsOut.ContinuousActions;
        continuous[0] = Input.GetAxis("Horizontal");
        continuous[1] = Input.GetAxis("Vertical");
    }
}
