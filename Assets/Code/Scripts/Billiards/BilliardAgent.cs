using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BilliardAgent : AgentDLC
{
    [SerializeField] private float action_delay;

    private bool turn_started;
    private bool set_position;
    private float time_passed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        turn_started = false;
        time_passed = 0;
    }

    private void Update()
    {
        if (turn_started)
        {
            time_passed += Time.deltaTime;
            if (time_passed > action_delay)
            {
                turn_started = false;
                RequestDecision();
            }
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        List<BallData> balls = BilliardSIngleton.instance.get_ball_data();
        BallData data = BilliardSIngleton.instance.get_cue_ball();

        sensor.AddObservation(data.get_ball().transform);
        sensor.AddObservation(data.get_ball().activeSelf);
        sensor.AddObservation(data.get_rigidbody().linearVelocity);

        for (int i = 0; i < balls.Count; i++)
        {
            sensor.AddObservation(balls[i].get_ball().transform);
            sensor.AddObservation(balls[i].get_ball().activeSelf);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        BilliardSIngleton.instance.shoot_ball(new Vector3(actions.ContinuousActions[0], 0, actions.ContinuousActions[1]), get_positive(actions.ContinuousActions[2]));
        
        if (set_position)
        {
            // BilliardSIngleton.instance.set_cue_ball_position();
        }
    }

    public void start_turn(GameObject game_object, bool new_set_position)
    {
        if (gameObject == game_object)
        {
            turn_started = true;
            set_position = new_set_position;
            time_passed = 0;
        }
        else
        {
            turn_started = false;
        }
    }
}
