using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
public class Tagger : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private MeshRenderer plane;
    [SerializeField] private Material success;
    [SerializeField] private Material fail;
    [Header("Movement Settings")]
    public float moveSpeed = 1f;
    private Rigidbody rb;
    public override void OnEpisodeBegin()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = new Vector3(Random.Range(-20, 0), 0.5f, Random.Range(-20, 0));
        target.position = new Vector3(Random.Range(0, 20), 0.5f, Random.Range(0, 20));

       
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.localPosition += new Vector3(moveX, 0.5f, moveZ) * Time.deltaTime * moveSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Runner"))
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
        continuous[0] = Input.GetAxisRaw("Horizontal");
        continuous[1] = Input.GetAxisRaw("Vertical");
    }
}

