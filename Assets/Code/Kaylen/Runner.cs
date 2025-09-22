using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Rigidbody))]
public class Runner : Agent
{
    [SerializeField] public Transform tagger;            // Reference to the seeker


    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float dangerRadius = 5f;




    private Rigidbody rb;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // prevent tumbling
    }

    public override void OnEpisodeBegin()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //transform.localPosition = new Vector3(Random.Range(-21, 21), 0.5f, Random.Range(-21, 21));
        //if (tagger != null)
        //{
        //    tagger.localPosition = new Vector3(Random.Range(-21, 21), 0.5f, Random.Range(-21, 21));
        //}
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);   // runner pos
        sensor.AddObservation(tagger.localPosition);      // tagger pos
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Movement
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        Vector3 move = new Vector3(moveX, 0, moveZ).normalized * moveSpeed;
        rb.MovePosition(transform.position + move * Time.fixedDeltaTime);

        // --- Distance-based penalty/reward ---
        float distance = Vector3.Distance(transform.position, tagger.position);

        // Punish heavily if the Tagger is very close
        if (distance < dangerRadius)
        {
            // The closer the Tagger, the stronger the penalty
            float penalty = Mathf.Lerp(-1f, 0f, distance / dangerRadius);
            AddReward(penalty * Time.deltaTime);
        }
        else
        {
            // Small positive reward for surviving outside the danger zone
            AddReward(0.01f * Time.deltaTime);
        }
    }
 

    private void OnCollisionEnter(Collision collision)
    {
       if (collision.collider.CompareTag("Wall"))
        {

            AddReward(-10f);   // big penalty for touching wall
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuous = actionsOut.ContinuousActions;
        continuous[0] = Input.GetAxisRaw("Horizontal"); // X movement
        continuous[1] = Input.GetAxisRaw("Vertical");   // Z movement
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dangerRadius);
    }
}
