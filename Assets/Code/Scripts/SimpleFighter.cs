using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

public class SimpleFighter : Agent
{
    [SerializeField] private List<GameObject> opponents;

    private float movespeed = 1;

    private Vector3 direction = Vector3.zero;

    private BehaviorParameters behaviour_parameters;

    private void Start()
    {
        behaviour_parameters = GetComponent<BehaviorParameters>();
        EventHandler.GotHit += reward_hit;
        EventHandler.GotKill += reward_kill;
    }

    private void OnDestroy()
    {
        EventHandler.GotHit -= reward_hit;
        EventHandler.GotKill -= reward_kill;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.rotation);

        for (int i = 0; i < opponents.Count; i++)
        {
            behaviour_parameters.BrainParameters.VectorObservationSize += 7;

            sensor.AddObservation(opponents[i].transform.localPosition);
            sensor.AddObservation(opponents[i].transform.rotation);
        }
    }

    //private void Update()
    //{
    //    //print(direction);

    //    transform.localPosition += direction * Time.deltaTime * movespeed;
    //    transform.forward = direction;
    //}

    public override void OnActionReceived(ActionBuffers actions)
    {
        direction.x = actions.ContinuousActions[0];
        direction.z = actions.ContinuousActions[1];
        direction.Normalize();

        transform.localPosition += direction * Time.deltaTime * movespeed;
        transform.forward = direction;

        AddReward(-0.01f);
    }

    public void reward_hit(GameObject hitter, GameObject target, float damage)
    {
        if (hitter == gameObject)
        {
            AddReward(1f);
        }
        else if (target == gameObject)
        {
            AddReward(-1f);
        }
    }

    public void reward_kill(GameObject killer, GameObject target)
    {
        if (killer == gameObject)
        {
            AddReward(5);
        }
        else if (target == gameObject)
        {
            AddReward(-5f);
        }
    }
}
