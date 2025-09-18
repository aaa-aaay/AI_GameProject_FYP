using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using System.Collections.Generic;

public class BilliardAgent : AgentDLC
{
    [SerializeField] float action_delay;
    [SerializeField] float table_length;
    [SerializeField] float table_width;
    
    float time_passed;
    bool turn_started;
    bool set_position;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        time_passed = 0;
        turn_started = false;
        set_position = false;

        base_start();

        EventHandler.StartTurn += turn_start;
        EventHandler.RestartGame += restart;

    }

    private void OnDestroy()
    {
        base_destroy();

        EventHandler.StartTurn -= turn_start;
        EventHandler.RestartGame -= restart;
    }

    // Update is called once per frame
    void Update()
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

    public override void CollectObservations(VectorSensor sensor)
    {
        Rigidbody temp = BilliardSingleton.instance.get_cue_ball(gameObject);
        List<Rigidbody> balls = BilliardSingleton.instance.get_balls(gameObject);

        sensor.AddObservation(BilliardSingleton.instance.get_score(gameObject));
        sensor.AddObservation(temp.transform.localPosition);
        sensor.AddObservation(set_position);

        for (int i = 0; i < balls.Count; i++)
        {
            sensor.AddObservation(balls[i].transform.localPosition);
            sensor.AddObservation(balls[i].isKinematic);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        BilliardSingleton.instance.shoot_ball(gameObject, actions.ContinuousActions[0], actions.ContinuousActions[1], get_positive(actions.ContinuousActions[2]));

        if (set_position)
        {
            BilliardSingleton.instance.set_position(gameObject, actions.ContinuousActions[3] * table_length * 0.5f, actions.ContinuousActions[4] * table_width * 0.5f);
        }
        turn_started = false;

        //print(gameObject.name + " shot ball");
    }

    public void turn_start(GameObject game_object, bool new_set_position)
    {
        if (gameObject == game_object)
        {
            //print(game_object.name + " started turn");
            time_passed = 0;
            turn_started = true;
            set_position = new_set_position;
            AddReward(-0.1f);
        }
    }

    public void restart(GameObject game_object)
    {
        if (gameObject == game_object)
        {
            time_passed = 0;
            turn_started = false;
            set_position = false;
            EndEpisode();
        }
    }
}
