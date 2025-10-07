using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Events;

public class DodgeballAgent : AgentDLC
{
    [SerializeField] private DodgeballInstance instance;
    [SerializeField] private Damageable damageable;

    [SerializeField] private UnityEvent<Vector2> Move;
    [SerializeField] private UnityEvent<Vector3> Rotate;

    private Vector3 start_position;

    private ShootInterface shoot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventHolder.OnTakeDamage += OnTakeDamage;
        EventHolder.OnKill += OnKill;
        EventHolder.OnRestart += OnRestart;

        start_position = transform.position;

        shoot = GetComponent<ShootInterface>();
    }

    // Update is called once per frame
    void Update()
    {
        RequestDecision();
    }

    protected override void OnRestart(GameObject player)
    {
        if (player == gameObject)
        {
            EndEpisode();
            transform.position = start_position;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        List<Damageable> players = instance.GetDamageablePlayers();
        List<Damageable> opponents = instance.GetDamageablePlayers();

        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.localRotation);
        sensor.AddObservation(damageable.GetHealth());
        sensor.AddObservation(shoot.CanShoot());

        players.Remove(damageable);
        opponents.Remove(damageable);

        for (int i = 0; i < players.Count; i++)
        {

        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector2 direction;
        Vector3 rotate;

        // Movement switch case
        switch (actions.DiscreteActions[0])
        {
            case 0:
                {
                    direction = Vector2.up;
                }
                break;
            case 1:
                {
                    direction = Vector2.down;
                }
                break;
            case 2:
                {
                    direction = Vector2.left;
                }
                break;
            case 3:
                {
                    direction = Vector2.right;
                }
                break;
            case 4:
                {
                    direction = Vector2.zero;
                    shoot.Shoot();
                }
                break;
            default:
                direction = Vector2.zero;
                break;
        }

        // Rotation switch case
        switch (actions.DiscreteActions[1])
        {
            case 0:
                {
                    rotate = new Vector3(0, -5, 0);
                }
                break;
            case 1:
                {
                    rotate = new Vector3(0, 5, 0);
                }
                break;
            default:
                rotate = Vector3.zero;
                break;
        }

        InvokeMove(direction);
        InvokeRotate(rotate);
    }

    private void InvokeMove(Vector2 direction)
    {
        if (Move != null)
        {
            Move.Invoke(direction);
        }
    }

    private void InvokeRotate(Vector3 rotate)
    {
        if (Rotate != null)
        {
            Rotate.Invoke(rotate);
        }
    }
}
