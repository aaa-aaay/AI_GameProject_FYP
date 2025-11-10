using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class RacingAgent : Agent
{
    [SerializeField] private RaceManager _manager;
    [SerializeField] private GameObject _car;

    private Rigidbody _sphere;
    private BetterCarMovement _carMovement;
    private GoalChecker _goalChecker;
    private WallFrictionHandler _wallFrictionHandler;
    private ResetCarPosition _carPositionResetter;
    private float _raceTimer;
    public float TimeToReachNextCheckpoint = 50f;

    private float _previousDistanceToCheckpoint;

    private Vector3 _lastPosition;
    private float _lastSpeed;

    public override void Initialize()
    {
        _carMovement = GetComponent<BetterCarMovement>();
        _goalChecker = _car.GetComponent<GoalChecker>();
        _sphere = _car.GetComponent<Rigidbody>();
        _carPositionResetter = _car.GetComponent<ResetCarPosition>();
        _wallFrictionHandler = _car.GetComponent<WallFrictionHandler>();

        //_manager.onRaceOver += HandleRaceOver;
        _goalChecker.OnRaceFinished += AiFinishedRace;
        _goalChecker.onCheckPointHit += HandleCPHit;

        _wallFrictionHandler.OnHitWall += HitWall;
        _wallFrictionHandler.OnWallStay += stayingOnWall;
    }

    public override void OnEpisodeBegin()
    {
        _raceTimer = TimeToReachNextCheckpoint;
        _carPositionResetter.ResetPos();
        _goalChecker.ResetCar();


        _previousDistanceToCheckpoint = Vector3.Distance(
            _car.transform.position,
            _goalChecker.GetCurrentCheckPoint().position);
    }

    private void Update()
    {
        _raceTimer -= Time.deltaTime;

        float dist = Vector3.Distance(_car.transform.position, _goalChecker.GetCurrentCheckPoint().position);
        float progress = _previousDistanceToCheckpoint - dist;
        AddReward(progress * 0.002f); // reward moving closer



        float currentSpeed = _sphere.linearVelocity.magnitude;
        //AddReward((currentSpeed - _lastSpeed) * 0.005f); // encourage acceleration
        //_lastSpeed = currentSpeed;

        // Penalize being too slow
        if (currentSpeed < 5f)
            AddReward(-0.005f);


        AddReward(currentSpeed / 30f * 0.02f);


        _previousDistanceToCheckpoint = dist;

        if (_raceTimer <= 0f)
        {
            AddReward(-1f);
            EndEpisode();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 toCheckpoint = _goalChecker.GetCurrentCheckPoint().position - _car.transform.position;
        Vector3 forward = _car.transform.forward;

        sensor.AddObservation(toCheckpoint.normalized);          // where to go
        sensor.AddObservation(Vector3.Dot(forward, toCheckpoint.normalized)); // alignment
        sensor.AddObservation(_sphere.linearVelocity.magnitude / 30f); // speed
        sensor.AddObservation(_sphere.linearVelocity.normalized); // direction of motion
    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        //Movement(actions.DiscreteActions);
        Movement(actions.ContinuousActions, actions.DiscreteActions);
        //rewardForGettingCloser();
        AddReward(-0.002f); //avoid stalling
    }



    private void HandleCPHit()
    {
        float checkpointSpeed = _sphere.linearVelocity.magnitude;
        _raceTimer = TimeToReachNextCheckpoint;
    }

    private void AiFinishedRace(string name, float timetaken)
    {
        AddReward(0.5f);
        EndEpisode();
    }



    private void Movement(ActionSegment<float> act, ActionSegment<int> disAct)
    {
        float horizontal = Mathf.Clamp(act[0], -1f, 1f);
        float vertical = Mathf.Clamp01(act[1]);
        Vector2 inputDir = new Vector2(horizontal, vertical);
        inputDir = Vector2.ClampMagnitude(inputDir, 1f); // normalize if needed


        _carMovement.MoveCar(inputDir);


        //if (Mathf.Abs(inputDir.x) > 0.01f)
        //{
        //    bool toDrift = disAct[0] == 1;
        //    if (disAct[0] == 0) toDrift = false;
        //    else if (disAct[0] == 1) toDrift = true;
        //    _carMovement.ToggleDrifting(toDrift, inputDir.x);
        //}

    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {

        var continuousActions = actionsOut.ContinuousActions;
        var DiscreteActions = actionsOut.DiscreteActions;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        continuousActions[0] = horizontal;
        continuousActions[1] = Input.GetKey(KeyCode.W)? 1f: 0f;

        //if(Input.GetKey(KeyCode.LeftShift))
        //{

        //    DiscreteActions[0] = 1;
        //}
        //else
        //{
        //    DiscreteActions[0] = 0;
        //}

    }


    private void HitWall()
    {
        AddReward(-0.05f);
    }
    private void stayingOnWall()
    {
        AddReward(-0.01f);
    }







    //public override void Heuristic(in ActionBuffers actionsOut)
    //{
    //    var discreteActionsOut = actionsOut.DiscreteActions;
    //    discreteActionsOut[0] = 0; // default: no input

    //    bool w = Input.GetKey(KeyCode.W);
    //    bool a = Input.GetKey(KeyCode.A);
    //    bool s = Input.GetKey(KeyCode.S);
    //    bool d = Input.GetKey(KeyCode.D);

    //    // 8-direction movement
    //    if (w && d) discreteActionsOut[0] = 2;        // forward-right
    //    else if (s && d) discreteActionsOut[0] = 4;   // back-right
    //    else if (s && a) discreteActionsOut[0] = 6;   // back-left
    //    else if (w && a) discreteActionsOut[0] = 8;   // forward-left
    //    else if (w) discreteActionsOut[0] = 1;        // forward
    //    else if (d) discreteActionsOut[0] = 3;        // right
    //    else if (s) discreteActionsOut[0] = 5;        // back
    //    else if (a) discreteActionsOut[0] = 7;        // left
    //}


    //private void Movement(ActionSegment<int> act)
    //{
    //    int directionAction = act[0];

    //    Vector2 inputDir = Vector2.zero;

    //    switch (directionAction)
    //    {
    //        case 0: inputDir = Vector2.zero; break;                       // do nothing
    //        case 1: inputDir = Vector2.up; break;                         // forward
    //        case 2: inputDir = new Vector2(1, 1).normalized; break;       // forward-right
    //        case 3: inputDir = Vector2.right; break;                      // right
    //        case 4: inputDir = new Vector2(1, -1).normalized; break;      // back-right
    //        case 5: inputDir = Vector2.down; break;                       // back
    //        case 6: inputDir = new Vector2(-1, -1).normalized; break;     // back-left
    //        case 7: inputDir = Vector2.left; break;                       // left
    //        case 8: inputDir = new Vector2(-1, 1).normalized; break;      // forward-left
    //    }

    //    _carMovement.MoveCar(inputDir);

    //}

}
