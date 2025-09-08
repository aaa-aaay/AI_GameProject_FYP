using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Integrations.Match3;
using Unity.MLAgents.Sensors;
using UnityEngine;
using static Racket;

public class BadmintonAgent : Agent
{

    [Header("Observation Refereneces")]
    [SerializeField] private Transform _shuttle;
    [SerializeField] private Transform _net;
    [SerializeField] private Transform _opponetTransform;

    [Header("Stats")]
    [SerializeField] private float _moveSpeed = 5.0f;
    [SerializeField] private float _offsetFromShuttle = 1.2f;
    [SerializeField] private float _hitRange = 2.0f;
    [SerializeField] private float minHeight = 0.3f;
    [SerializeField] private float maxHeight = 5.0f;


    [Header("Other References")]
    [SerializeField] private RacketSwing _racketSwing;



    private Vector3 startPosition;

    public override void Initialize()
    {
        startPosition = transform.localPosition;
    }

    public override void OnEpisodeBegin()
    {
        

        transform.localPosition = startPosition + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));


    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(_shuttle.transform.localPosition);
        sensor.AddObservation(_net.localPosition);
        sensor.AddObservation(_shuttle.localPosition - transform.localPosition); // relative pos to shuttle
        sensor.AddObservation(_net.localPosition - transform.localPosition);   // relative pos to net

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int shotType = actions.DiscreteActions[0];

        MoveAgent(actions.DiscreteActions);
        Swinging();


    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var action = act[0];

        //movement
        Vector3 movement = Vector3.zero;
        Vector3 dirToNet = (_net.position - _shuttle.position).normalized;

        switch (action)
        {
            case 0: movement = Vector3.zero; break;          // do nothing
            case 1: movement = Vector3.left; break;          // move left
            case 2: movement = Vector3.right; break;         // move right
            case 3: movement = Vector3.forward; break;       // move forward
            case 4: movement = Vector3.back; break;          // move backward
        }

        transform.position += movement * _moveSpeed * Time.deltaTime;

        // Prevent crossing the net (keep the "side" rule but no offset to shuttle)
        if (transform.position.z > _net.position.z)
        {
            transform.position = new Vector3(transform.position.x,
            transform.position.y, Mathf.Max(transform.position.z, _net.position.z + 0.2f));
        }
        else
        {
            transform.position = new Vector3(transform.position.x,
               transform.position.y, Mathf.Min(transform.position.z, _net.position.z - 0.2f));
        }
           

    }


    public void Swinging()
    {
        // Direction toward net
        Vector3 dirToNet = (_net.position - _shuttle.position).normalized;

        // Ideal position: slightly "behind" shuttle relative to net
        Vector3 idealPos = _shuttle.position - dirToNet * _offsetFromShuttle;

        // Distance to that spot
        float distToIdeal = Vector3.Distance(transform.position, idealPos);

        // --- Step 1: Proximity reward ---
        // Give a stronger, non-linear reward for closing in
        float proximityReward = Mathf.Exp(-distToIdeal); // exponential decay
        AddReward(proximityReward * 0.1f); // scale factor

        // --- Step 2: Bonus for getting close --- 
        if (distToIdeal < _hitRange)
        {
            AddReward(+0.5f);  // strong encouragement to reach range
        }

        // --- Step 3: Height condition ---
        bool inHeight = _shuttle.position.y > minHeight && _shuttle.position.y < maxHeight;
        if (distToIdeal < _hitRange && inHeight)
        {
            AddReward(+1.0f);  // jackpot for being ready to hit
            Debug.Log("Perfect Position");
        }

        // --- Step 4: Penalty for being far away ---
        if (distToIdeal > _hitRange * 3f)
        {
            AddReward(-0.1f);
        }



    }




}
