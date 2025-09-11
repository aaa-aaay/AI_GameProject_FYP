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
    [SerializeField] private float _hitRange = 2.0f;

    [SerializeField] private float offsetFromShuttle = 1.6f;
    [SerializeField] private float minHeight = 0.5f;
    [SerializeField] private float maxHeight = 3.0f;


    [Header("Other References")]
    [SerializeField] private BadmintionGameManager _gameManager;
    [SerializeField] private RacketSwing _racketSwing;



    private Vector3 startPosition;
    private float _prevDistToShuttle;



    public override void Initialize()
    {
        startPosition = transform.localPosition;

    }

    public override void OnEpisodeBegin()
    {
        

        transform.localPosition = startPosition + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));


    }

    private void Update()
    {


        if(_racketSwing.racketSwinging)
        {

        }

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(_shuttle.transform.localPosition);
        sensor.AddObservation(_shuttle.localPosition - transform.localPosition);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        MoveAgent(actions.DiscreteActions);
        Swinging(actions.DiscreteActions);




        AddReward(-0.001f); // small time penalty to avoid stalling


    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var action = act[0];



        Vector3 dirToNet = (_net.position - _shuttle.position).normalized;
        Vector3 desiredPos = _shuttle.position - dirToNet * offsetFromShuttle;




        // Distance reward
        float currentDist = Vector3.Distance(transform.localPosition, desiredPos);
        float distChange = _prevDistToShuttle - currentDist;

        // Reward for moving closer, penalty for moving away
        if (currentDist > _prevDistToShuttle)
        {
            AddReward(-distChange * 0.1f);
        }
        else
        {
            AddReward(distChange * 0.1f);

        }

        _prevDistToShuttle = currentDist;




        //movement
        Vector3 movement = Vector3.zero;
        //Vector3 dirToNet = (_net.position - _shuttle.position).normalized;

        switch (action)
        {
            case 0: movement = Vector3.zero; break;          // do nothing
            case 1: movement = Vector3.left; break;          // move left
            case 2: movement = Vector3.right; break;         // move right
            case 3: movement = Vector3.forward; break;       // move forward
            case 4: movement = Vector3.back; break;          // move backward
        }

        transform.position += movement * _moveSpeed * Time.deltaTime;



        // Prevent crossing the net
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


    public void Swinging(ActionSegment<int> act)
    {

        //float distToIdeal = Vector3.Distance(transform.position, _shuttle.transform.position);
       
        bool inRange = Vector3.Distance(transform.position, _shuttle.position) < _hitRange;
        bool inHeight = _shuttle.position.y > minHeight && _shuttle.position.y < maxHeight;


        if (inRange && inHeight)
        {

            var swingingAction = act[1];
            Debug.Log("In Range");
            SwingRacket(swingingAction);
            AddReward(1.0f);
        }
        //else
        //{
        //    AddReward(0.01f * -distToIdeal);
        //}

        



    }


    private void SwingRacket(int choice)
    {
        if (_racketSwing.racketSwinging) return;


        if (choice == 0)
            _racketSwing.StartSwing(Racket.ShotType.Smash);
        else if (choice == 1)
            _racketSwing.StartSwing(Racket.ShotType.Drop);
        else
            _racketSwing.StartSwing(Racket.ShotType.Lob);
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {

        var discreteActionsOut = actionsOut.DiscreteActions;

        discreteActionsOut[0] = 0; // don't move - do nothing!

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            discreteActionsOut[0] = 2;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            discreteActionsOut[0] = 3;
        }
        else if( Input.GetKey(KeyCode.DownArrow))
        {
            discreteActionsOut[0] = 4;
        }

    }

}
