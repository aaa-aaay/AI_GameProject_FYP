using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BilliardAgent : AgentDLC
{
    [SerializeField] private float action_delay;
    [SerializeField] private float table_length;
    [SerializeField] private float table_width;

    private bool turn_started;
    private bool set_position;
    private float time_passed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        turn_started = false;
        set_position = false;
        time_passed = 0;

        EventHandler.StartTurn += start_turn;
        EventHandler.EndScenario += restart;

        base_start();
    }

    private void Update()
    {
        if (turn_started)
        {
            time_passed += Time.deltaTime;
            if (time_passed > action_delay)
            {
                RequestDecision();
            }
        }
    }

    public void restart()
    {
        EndEpisode();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        List<BallData> balls = BilliardSingleton.instance.get_balls();
        BallData data = BilliardSingleton.instance.get_cue_ball();

        sensor.AddObservation(data.get_ball().transform);
        sensor.AddObservation(data.get_rigidbody().linearVelocity);

        for (int i = 0; i < balls.Count; i++)
        {
            sensor.AddObservation(balls[i].get_ball().transform);
            sensor.AddObservation(balls[i].get_rigidbody().linearVelocity);
            sensor.AddObservation(balls[i].get_ball().transform.position.y >= 0);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (set_position)
        {
            BilliardSingleton.instance.set_cue_ball_position(new Vector2(actions.ContinuousActions[3] * table_length / 2, actions.ContinuousActions[4] * table_width / 2));
        }

        Vector3 dir = new Vector3(actions.ContinuousActions[0], 0, actions.ContinuousActions[1]);

        BilliardSingleton.instance.shoot_ball(dir, get_positive(actions.ContinuousActions[2]));

        turn_started = false;
        set_position = false;
    }

    public void start_turn(GameObject game_object, bool new_set_position)
    {
        if (gameObject == game_object)
        {
            print(game_object.name + " turn started");
            turn_started = true;
            set_position = new_set_position;
            time_passed = 0;
        }
    }
}
