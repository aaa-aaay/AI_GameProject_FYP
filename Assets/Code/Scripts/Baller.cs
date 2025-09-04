using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Baller : AgentDLC
{
    [SerializeField] Ball ball;
    [SerializeField] List<GameObject> balls;
    [SerializeField] Vector2 table_measurements;

    private bool can_set_position;
    private bool can_act = false;
    private float delay = 1f;
    private float time_passed = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        base.Awake();
        base_start();

        EventHandler.TurnStart += shoot_ball;
        EventHandler.EndScenario += EndEpisode;
        EventHandler.EndScenario += reset_ai;
        EventHandler.Scored += score_reward;

        can_set_position = false;
    }

    private void Update()
    {
        if (can_act)
        {
            time_passed += Time.deltaTime;
            if (time_passed > delay)
            {
                RequestDecision();
                can_act = false;
            }        
        }
    }

    private void OnDestroy()
    {
        EventHandler.TurnStart -= shoot_ball;
        EventHandler.EndScenario -= EndEpisode;
        EventHandler.EndScenario -= reset_ai;
        EventHandler.Scored -= score_reward;
    }

    public void reset_ai()
    {
        can_set_position = false;
    }

    public override void OnEpisodeBegin()
    {
        can_set_position = false;
    }

    public void score_reward(GameObject scorer)
    {
        if (scorer == gameObject)
        {
            AddReward(5f);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(ball.transform.position);

        for (int i = 0; i < balls.Count; i++)
        {
            sensor.AddObservation(balls[i].transform.position);
        }
    }

    public void shoot_ball(GameObject player, bool set_position)
    {
        if (player == gameObject)
        {
            //RequestDecision();
            can_act = true;
            can_set_position = set_position;
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        ball.shoot(new Vector2(actions.ContinuousActions[0], actions.ContinuousActions[1]), actions.DiscreteActions[0]);

        if (can_set_position)
        {
            ball.set_position(new Vector2(actions.ContinuousActions[2] * table_measurements.x * 0.5f, actions.ContinuousActions[3] * table_measurements.y * 0.5f));
            can_set_position = false;
        }

        print("Shot ball");
    }
}
