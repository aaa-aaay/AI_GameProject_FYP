using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class CatchAI : AgentDLC
{
    [SerializeField] private CatchInstance instance;

    [SerializeField] private UnityEvent<Vector2> Move;
    [SerializeField] private UnityEvent Jump;

    private Jump jumper;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventHolder.OnRestart += OnRestart;
        EventHolder.OnHit += OnHit;

        jumper = GetComponent<Jump>();
    }

    private void OnDestroy()
    {
        EventHolder.OnRestart -= OnRestart;
        EventHolder.OnHit -= OnHit;
    }

    // Update is called once per frame
    void Update()
    {
        RequestDecision();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector2 direction = Vector3.zero;

        switch (actions.DiscreteActions[0])
        {
            case 0:
                direction = Vector2.left;
                break;
            case 1:
                direction = Vector2.right;
                break;
            default:
                break;
        }

        Move?.Invoke(direction);

        if (actions.DiscreteActions[1] == 1)
        {
            if (!jumper.CanJump())
            {
                AddReward(-0.02f);
            }
            Jump?.Invoke();
        }
    }

    protected override void OnRestart()
    {
        EndEpisode();
    }

    protected override void OnHit(GameObject hitter, GameObject target, float damage)
    {
        if (hitter == gameObject)
        {
            AddReward(0.01f);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        List<CatchData> players = instance.GetData();

        sensor.AddObservation(transform.localPosition);

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetPlayer() != gameObject)
            {
                sensor.AddObservation(players[i].GetPlayer().transform.localPosition);
            }
        }

        List<CatchItem> items = instance.GetItems();

        for (int i = 0; i < items.Count; i++)
        {
            sensor.AddObservation(items[i].transform.localPosition);
            sensor.AddObservation(items[i].gameObject.activeSelf);
        }
    }
}
