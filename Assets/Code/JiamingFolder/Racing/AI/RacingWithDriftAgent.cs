using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class RacingWithDriftAgent : Agent
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
        AddReward(progress * 0.05f); // reward moving closer
        if (progress < 0)
        {
            AddReward(progress * 0.04f);
            Debug.Log("Driving Backwards");
        }



        float currentSpeed = _sphere.linearVelocity.magnitude;

        // Penalize being too slow
        if (currentSpeed < 7f)
            AddReward(-0.02f);


        AddReward(currentSpeed / 30f * 0.02f);


        _previousDistanceToCheckpoint = dist;

        if (_raceTimer <= 0f)
        {
            AddReward(-5.0f);
            EndEpisode();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 toCheckpoint = _goalChecker.GetCurrentCheckPoint().position - _car.transform.position;
        Vector3 forward = _car.transform.forward;

        sensor.AddObservation(toCheckpoint.normalized);          
        sensor.AddObservation(Vector3.Dot(forward, toCheckpoint.normalized)); 
        sensor.AddObservation(_sphere.linearVelocity.magnitude / 30f); 
        sensor.AddObservation(_sphere.linearVelocity.normalized); 
    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        Movement(actions.ContinuousActions, actions.DiscreteActions);
        AddReward(-0.002f); //avoid stalling
    }



    private void HandleCPHit()
    {
        AddReward(0.15f);
        float checkpointSpeed = _sphere.linearVelocity.magnitude;
        _raceTimer = TimeToReachNextCheckpoint;
    }

    private void AiFinishedRace(string name, float timetaken)
    {
        AddReward(3.0f);
        if (_manager.isDebugMood)
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
        continuousActions[1] = Input.GetKey(KeyCode.W) ? 1f : 0f;

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
        AddReward(-0.15f);
    }
    private void stayingOnWall()
    {
        AddReward(-0.01f);
    }
}
