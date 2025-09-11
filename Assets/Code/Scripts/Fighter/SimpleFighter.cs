using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEngine.Events;

public class SimpleFighter : AgentDLC
{
    [SerializeField] private List<GameObject> opponents;

    [SerializeField] UnityEvent<Vector2> Move;
    [SerializeField] UnityEvent Dash;
    [SerializeField] UnityEvent LeftPunch;
    [SerializeField] UnityEvent RightPunch;

    private void Start()
    {
        base_start();
        EventHandler.TookDamage += reward_hit;
        EventHandler.GotKill += reward_kill;
        EventHandler.EndScenario += OnEpisodeBegin;
    }

    private void OnDestroy()
    {
        EventHandler.TookDamage -= reward_hit;
        EventHandler.GotKill -= reward_kill;
        EventHandler.EndScenario -= OnEpisodeBegin;
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(-10, 11), -0.5f, Random.Range(-10, 11));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation((int) TeamSingleton.instance.get_team(gameObject));
        sensor.AddObservation(TeamSingleton.instance.get_health(gameObject));

        for (int i = 0; i < opponents.Count; i++)
        {
            sensor.AddObservation(opponents[i].transform.position);
            sensor.AddObservation(opponents[i].transform.rotation);
            sensor.AddObservation((int) TeamSingleton.instance.get_team(opponents[i]));
            sensor.AddObservation(TeamSingleton.instance.get_health(opponents[i]));
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector2 direction = new Vector2(actions.ContinuousActions[0], actions.ContinuousActions[1]);

        Move.Invoke(direction);

        AddReward(-0.01f);

        if (direction != Vector2.zero)
        {
            for (int i = 0; i < opponents.Count; i++)
            {
                if (!TeamSingleton.instance.check_same_team(gameObject, opponents[i]))
                {
                    if (Vector3.Angle(transform.forward, opponents[i].transform.position - transform.position) < 10)
                    {
                        AddReward(0.02f);
                        break;
                    }
                }
            }
        }

        switch (actions.DiscreteActions[0])
        {
            case 0:
                Dash.Invoke();
                break;
            case 1:
                LeftPunch.Invoke();
                break;
            case 2:
                RightPunch.Invoke();
                break;
            default:
                break;
        }
    }

    public void reward_hit(GameObject hitter, GameObject target, float damage)
    {
        if (hitter.transform.root.gameObject == gameObject)
        {
            AddReward(2f);
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
            AddReward(10);
        }
        else if (target == gameObject)
        {
            //transform.localPosition = new Vector3(Random.Range(-10, 11), -0.5f, Random.Range(-10, 11));
            AddReward(-5f);
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (opponents.Contains(collision.gameObject))
        {
            if (Vector3.Angle(transform.forward, collision.transform.position - transform.position) < 10)
            {
                AddReward(0.5f);
            }
            else
            {
                AddReward(0.05f);
            }
        }
    }
}
