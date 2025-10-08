using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BadmintonNewAgent : Agent
{
    [Header("Observation Refereneces")]
    [SerializeField] private Transform _shuttle;
    [SerializeField] private Transform _net;
    [SerializeField] private Transform _opponetTransform;

    [Header("Stats")]
    [SerializeField] private float _moveSpeed = 5.0f;
    [SerializeField] private float _moveSpeedSlow = 5.0f;
    [SerializeField] private float _moveSpeedVerySlow = 5.0f;
    [SerializeField] private float _offsetFromNet = 0.1f;
    [SerializeField] private float _targetRangeFromShutter = 3.0f;

    [Header("Other References")]
    [SerializeField] private BadmintionGameManager _gameManager;
    [SerializeField] private RacketSwing _racketSwing;
    [SerializeField] private Racket _racket;
    [SerializeField] private Transform _shotMarker;

    private BadmintionMovement _movement;
    private BadmintonStamina _stamina;

    private Vector3 _startPosition;
    private float _prevDistToShuttle;

    private Vector3 _prevShuttlePos;
    private Vector3 _shuttleVelocity;

    private float _finalMoveSpeed;


    public override void Initialize()
    {
        _startPosition = transform.localPosition;
        _racket.OnHitShutter += RewardForHiting;
        _racket.OnMissShutter += PunishForMissing;
        _gameManager.OnGameOver += HandleGameOver;
        _gameManager.OnPlayer1Score += AIScores;
        _gameManager.OnPlayer2Score += EnemyScores;

        _movement = GetComponent<BadmintionMovement>();
        _stamina = GetComponent<BadmintonStamina>();
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = _startPosition;
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
        //sensor.AddObservation(_stamina.GetStamina());
        //sensor.AddObservation(_finalMoveSpeed);

        // --- New useful observations ---
        sensor.AddObservation(_shuttleVelocity); // direction + speed
        sensor.AddObservation(Vector3.Distance(transform.localPosition, _shuttle.localPosition));

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        AdjustMovement();
        MoveAgent(actions.DiscreteActions);
        StaminaRewards();
        AddReward(-0.01f); // small time penalty to avoid stalling
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var directionAction = act[0];
        var action = act[1];
        var swingingAction = act[2];



        //movement
        Vector3 moveDir = Vector3.zero;


        switch (directionAction)
        {
            case 0: moveDir = Vector3.zero; break;                        // do nothing
            case 1: moveDir = Vector3.forward; break;                     // forward
            case 2: moveDir = new Vector3(1, 0, 1).normalized; break;     // forward-right
            case 3: moveDir = Vector3.right; break;                       // right
            case 4: moveDir = new Vector3(1, 0, -1).normalized; break;    // back-right
            case 5: moveDir = Vector3.back; break;                        // back
            case 6: moveDir = new Vector3(-1, 0, -1).normalized; break;   // back-left
            case 7: moveDir = Vector3.left; break;                        // left
            case 8: moveDir = new Vector3(-1, 0, 1).normalized; break;    // forward-left
        }


        switch (action)
        {
            case 0: SetMovement(moveDir); break;       // walk
            case 1: SwingRacket(swingingAction); break;    //swing the racket           
            case 2: Dash(moveDir); break;// dash               
        }


        if (_startPosition.z > _net.localPosition.z)
        {
            if (transform.localPosition.z < _net.localPosition.z)
            {
                Vector3 corrected = transform.localPosition;
                corrected.z = _net.localPosition.z + _offsetFromNet;
                transform.localPosition = corrected;
            }
        }
        else
        {
            if (transform.localPosition.z > _net.localPosition.z)
            {
                Vector3 corrected = transform.localPosition;
                corrected.z = _net.localPosition.z - _offsetFromNet;
                transform.localPosition = corrected;
            }
        }

        MovementRewards();

    }

    private void SwingRacket(int choice)
    {
        if (_racketSwing.racketSwinging) return;

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
            _racketSwing.StartSwing(Racket.ShotType.Lob, 2);
    }

    private void Dash(Vector3 dir)
    {
        _movement.Dash(dir);
        AddReward(-0.01f);
        Debug.Log("Dashed");
        //if (_stamina.UseStamina(BadmintonStamina.actions.Dash))
        //{
        //    _movement.Dash(dir);
        //}
        //else
        //{
        //    //punish the AI for dashing and wasting stamina;
        //    AddReward(-0.1f);
        //}
        
    }

    private void AdjustMovement()
    {
        //if (_stamina.GetStamina() < _stamina.GetStaminaLimit(1))
        //{
        //    _finalMoveSpeed = _moveSpeedSlow;
        //    AddReward(-0.001f);
        //    if (_stamina.GetStamina() < _stamina.GetStaminaLimit(2))
        //    {
        //        _finalMoveSpeed = _moveSpeedVerySlow;
        //        AddReward(-0.003f);
        //    }
        //}
        //else
        //{
        //    _finalMoveSpeed = _moveSpeed;
        //}

        _finalMoveSpeed = _moveSpeed;
    }

    private void SetMovement(Vector3 dir)
    {
        if (dir == Vector3.zero)
        {
            AddReward(0.01f);
            //_stamina.UseStamina(BadmintonStamina.actions.Rest);
            _movement.Walk(false);
            Debug.Log("resting");
        }
        else {
            //_stamina.UseStamina(BadmintonStamina.actions.Running);
            _movement.Walk(true);
        } 

        transform.localPosition += dir * _finalMoveSpeed * Time.deltaTime;
    }

    private void Update()
    {
        // Compute pseudo-velocity manually
        _shuttleVelocity = (_shuttle.localPosition - _prevShuttlePos) / Time.deltaTime;
        _prevShuttlePos = _shuttle.localPosition;
    }

    private void MovementRewards()
    {
        Vector3 dirToNet = (_net.localPosition - _shuttle.localPosition).normalized;
        Vector3 desiredPos = _shuttle.localPosition - dirToNet;


        // Distance reward
        float currentDist = Vector3.Distance(transform.localPosition, desiredPos);

        if (currentDist < _prevDistToShuttle) AddReward(0.05f);
        else AddReward(-0.05f);

        _prevDistToShuttle = currentDist;

    }

    private void StaminaRewards()
    {
        //AddReward(_stamina.GetStamina() * 0.0001f);

        //if (_stamina.GetStamina() < _stamina.GetStaminaLimit(1))
        //{
        //    AddReward(-0.001f);
        //    if (_stamina.GetStamina() < _stamina.GetStaminaLimit(2))
        //    {
        //        AddReward(-0.003f);
        //    }
        //}

    }
    private void RewardForHiting()
    {
        AddReward(1.5f);
    }
    private void PunishForMissing()
    {
        AddReward(-0.3f);
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
