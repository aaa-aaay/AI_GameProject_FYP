using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Integrations.Match3;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BadmintonAgent : Agent
{

    [Header("Observation Refereneces")]
    [SerializeField] private Transform _shuttle;
    [SerializeField] private Transform _net;
    [SerializeField] private Transform _opponetTransform;

    [Header("Stats")]
    [SerializeField] private float _moveSpeed = 5.0f;

    [SerializeField] private float offsetFromShuttle = 1.6f;


    [Header("Other References")]
    [SerializeField] private BadmintionGameManager _gameManager;
    [SerializeField] private RacketSwing _racketSwing;
    [SerializeField] private Racket _racket;
    [SerializeField] private Transform _shotMarker;



    private Vector3 _startPosition;
    private float _prevDistToShuttle;

    private Vector3 _prevShuttlePos;
    private Vector3 _shuttleVelocity;



    public override void Initialize()
    {
        _startPosition = transform.localPosition;
        _racket.OnHitShutter += RewardForHiting;
        _racket.OnMissShutter += PunishForMissing;
        _gameManager.OnGameOver += HandleGameOver;
        _gameManager.OnPlayer1Score += AIScores;
        _gameManager.OnPlayer2Score += EnemyScores;


    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = _startPosition + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        _prevDistToShuttle = float.PositiveInfinity; // reset each episode


        // Reset previous position for velocity calc
        _prevShuttlePos = _shuttle.localPosition;
        _shuttleVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(_shuttle.transform.localPosition);
        sensor.AddObservation(_shuttle.localPosition - transform.localPosition);
        sensor.AddObservation(_gameManager.player1Score);
        sensor.AddObservation(_gameManager.player2Score);
        sensor.AddObservation(_opponetTransform.localPosition);
        sensor.AddObservation(_shotMarker.localPosition);

        // --- New useful observations ---
        sensor.AddObservation(_shuttleVelocity); // direction + speed
        sensor.AddObservation(Vector3.Distance(transform.localPosition, _shuttle.localPosition));

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        MoveAgent(actions.DiscreteActions);



        AddReward(-0.001f); // small time penalty to avoid stalling


    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var action = act[0];
        var swingingAction = act[1];


        //movement
        Vector3 movement = Vector3.zero;

        switch (action)
        {
            case 0: movement = Vector3.zero; break;          // do nothing
            case 1: movement = Vector3.left; break;          // move left
            case 2: movement = Vector3.right; break;         // move right
            case 3: movement = Vector3.forward; break;       // move forward
            case 4: movement = Vector3.back; break;
            case 5: SwingRacket(swingingAction); break;// swing the racket to try and hit the shutter
        }

        //transform.position += movement * _moveSpeed * Time.deltaTime;

        transform.localPosition += movement * _moveSpeed * Time.deltaTime;





        // Prevent crossing the net
        if (transform.localPosition.z > _net.localPosition.z)
        {
            Vector3 corrected = transform.localPosition;
            corrected.z = _net.localPosition.z - 0.2f; // stay just before the net
            transform.localPosition = corrected;
        }

        MovementRewards();

    }

    private void MovementRewards()
    {

        Vector3 dirToNet = (_net.localPosition - _shuttle.localPosition).normalized;
        Vector3 desiredPos = _shuttle.localPosition - dirToNet * offsetFromShuttle;


        // Distance reward
        float currentDist = Vector3.Distance(transform.localPosition, desiredPos);

        if (currentDist < _prevDistToShuttle) AddReward(0.05f);
        else AddReward(-0.05f);

        _prevDistToShuttle = currentDist;

    }


    private void SwingRacket(int choice)
    {
        if (_racketSwing.racketSwinging) return;


        //1 to hit right, 2 to hit left

        if (choice == 0)
            _racketSwing.StartSwing(Racket.ShotType.Smash, 1);
        else if (choice == 1)
            _racketSwing.StartSwing(Racket.ShotType.Drop, 1);
        else if (choice == 2)
            _racketSwing.StartSwing(Racket.ShotType.Lob, 1);
        else if (choice == 3)
            _racketSwing.StartSwing(Racket.ShotType.Smash, 2);
        else if (choice == 4)
            _racketSwing.StartSwing(Racket.ShotType.Drop, 2);
        else if (choice == 5)
            _racketSwing.StartSwing(Racket.ShotType.Smash, 2);
    }


    private void Update()
    {
        // Compute pseudo-velocity manually
        _shuttleVelocity = (_shuttle.localPosition - _prevShuttlePos) / Time.deltaTime;
        _prevShuttlePos = _shuttle.localPosition;
    }

    private void RewardForHiting()
    {
        AddReward(1.0f);
    }
    private void PunishForMissing()
    {
        AddReward(-0.5f);
    }
    private void AIScores()
    {
        AddReward(3.0f);
    }

    private void EnemyScores()
    {
        AddReward(-3.0f);
    }

    private void HandleGameOver()
    {
        EndEpisode();
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
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            discreteActionsOut[0] = 4;
        }

    }

}
