using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(Rigidbody))]
public class Runner : Agent
{
    [SerializeField] private Transform tagger;            // Reference to the seeker


    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float jumpForce = 5f;
    public float dangerRadius = 5f;

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

        // Reset positions
        transform.localPosition = new Vector3(Random.Range(-3f, 3f), 0.5f, Random.Range(-3f, 3f));
        if (tagger != null)
        {
            tagger.localPosition = new Vector3(Random.Range(-3f, 3f), 0.5f, Random.Range(-3f, 3f));
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);   // runner pos
        sensor.AddObservation(tagger.localPosition);      // tagger pos
        sensor.AddObservation(isGrounded ? 1 : 0);        // grounded check
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Movement
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        Vector3 move = new Vector3(moveX, 0, moveZ).normalized * moveSpeed;
        rb.MovePosition(transform.position + move * Time.fixedDeltaTime);

        float currentSpeed = moveSpeed;
        if (!isGrounded)
            currentSpeed *= 1.2f;

        // Jump
        float jump = actions.ContinuousActions[2];
        if (jump > 0.5f && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        // Distance penalty if tagger is close
        float distance = Vector3.Distance(transform.position, tagger.position);
        if (distance < dangerRadius)
        {
            float penalty = Mathf.Lerp(-1f, 0f, distance / dangerRadius);
            AddReward(penalty * Time.deltaTime);
           
        }
        else
        {
            AddReward(0.01f); // small survival reward
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        else if (collision.collider.CompareTag("Wall"))
        {
            AddReward(-1f);   // big penalty for touching wall
            EndEpisode();     // reset episode after mistake
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuous = actionsOut.ContinuousActions;
        continuous[0] = Input.GetAxisRaw("Horizontal"); // X movement
        continuous[1] = Input.GetAxisRaw("Vertical");   // Z movement
        continuous[2] = Input.GetButton("Jump") ? 1f : 0f; // Jump action
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dangerRadius);
    }
}
