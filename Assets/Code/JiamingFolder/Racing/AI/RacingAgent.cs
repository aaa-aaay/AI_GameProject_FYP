using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class RacingAgent : Agent
{
    private BetterCarMovement _carMovement;
    [SerializeField] private GoalChecker _goalChecker;
    [SerializeField] private Rigidbody _sphere;

    private float _raceTimer;
    public Transform _currentCheckPoint;




    public override void Initialize()
    {
        _carMovement = GetComponent<BetterCarMovement>();
        _goalChecker.onCheckPointHit += HandleCPHit;
    }

    public override void OnEpisodeBegin()
    {

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(_sphere.linearVelocity);
        sensor.AddObservation(_sphere.linearVelocity.magnitude);

        sensor.AddObservation(transform.forward);   
        sensor.AddObservation(transform.right);        
        sensor.AddObservation(transform.up);
        
        sensor.AddObservation(_currentCheckPoint.position);    

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Movement(actions.DiscreteActions);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 0; // default: no input

        bool w = Input.GetKey(KeyCode.W);
        bool a = Input.GetKey(KeyCode.A);
        bool s = Input.GetKey(KeyCode.S);
        bool d = Input.GetKey(KeyCode.D);

        // 8-direction movement
        if (w && d) discreteActionsOut[0] = 2;        // forward-right
        else if (s && d) discreteActionsOut[0] = 4;   // back-right
        else if (s && a) discreteActionsOut[0] = 6;   // back-left
        else if (w && a) discreteActionsOut[0] = 8;   // forward-left
        else if (w) discreteActionsOut[0] = 1;        // forward
        else if (d) discreteActionsOut[0] = 3;        // right
        else if (s) discreteActionsOut[0] = 5;        // back
        else if (a) discreteActionsOut[0] = 7;        // left
    }


    private void Movement(ActionSegment<int> act)
    {
        int directionAction = act[0];

        Vector2 inputDir = Vector2.zero;

        switch (directionAction)
        {
            case 0: inputDir = Vector2.zero; break;                       // do nothing
            case 1: inputDir = Vector2.up; break;                         // forward
            case 2: inputDir = new Vector2(1, 1).normalized; break;       // forward-right
            case 3: inputDir = Vector2.right; break;                      // right
            case 4: inputDir = new Vector2(1, -1).normalized; break;      // back-right
            case 5: inputDir = Vector2.down; break;                       // back
            case 6: inputDir = new Vector2(-1, -1).normalized; break;     // back-left
            case 7: inputDir = Vector2.left; break;                       // left
            case 8: inputDir = new Vector2(-1, 1).normalized; break;      // forward-left
        }

        _carMovement.MoveCar(inputDir);


    }

    private void HandleCPHit(Transform cpTransform)
    {
        _currentCheckPoint = cpTransform;
    }

}
