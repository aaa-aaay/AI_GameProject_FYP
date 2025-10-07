using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class ProjectileAgent : AgentDLC
{
    [SerializeField] private List<SimpleDamageable> opponents;
    [SerializeField] private List<Rigidbody> balls;
    [SerializeField] private ShootProjectile shoot;
    [SerializeField] private LayerMask layers;

    [SerializeField] UnityEvent<Vector2> Move;
    [SerializeField] UnityEvent<Vector3> Rotate;
    [SerializeField] UnityEvent Shoot;

    private SimpleDamageable self;
    private Vector3 start_position;

    private void Start()
    {
        base_start();
        EventHandler.TookDamage += reward_hit;
        EventHandler.GotKill += reward_kill;
        EventHandler.EndScenario += OnEpisodeBegin;
        self = GetComponent<SimpleDamageable>();

        start_position = transform.position;
    }

    private void OnDestroy()
    {
        base_destroy();
        EventHandler.TookDamage -= reward_hit;
        EventHandler.GotKill -= reward_kill;
        EventHandler.EndScenario -= OnEpisodeBegin;
    }

    public override void OnEpisodeBegin()
    {
        transform.position = start_position;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.localRotation);
        sensor.AddObservation(self.get_health());
        sensor.AddObservation(shoot.get_time_left());

        for (int i = 0; i < opponents.Count; i++)
        {
            sensor.AddObservation(opponents[i].transform.localPosition);
            sensor.AddObservation(opponents[i].transform.localRotation);
            sensor.AddObservation(opponents[i].get_health());
        }

        for (int i = 0; i < balls.Count; i++)
        {
            sensor.AddObservation(balls[i].transform.localPosition);
            sensor.AddObservation(balls[i].linearVelocity);
            sensor.AddObservation(balls[i].gameObject.activeSelf);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector2 direction = new Vector2(actions.ContinuousActions[0], actions.ContinuousActions[1]);

        switch (actions.DiscreteActions[0])
        {
            case 0:
                if (Move != null)
                {
                    Move.Invoke(direction);
                }
                break;
            case 1:
                if (Rotate != null)
                {
                    Rotate.Invoke(new Vector3(direction.x, 0, direction.y));
                }
                break;
            case 2:
                if (!shoot.gameObject.activeSelf)
                {
                    for (int i = 0; i < opponents.Count; i++)
                    {
                        if (Vector3.Angle(transform.forward, opponents[i].transform.position - transform.position) < 5)
                        {
                            AddReward(1);
                        }
                    }
                }
                Shoot.Invoke();
                break;
            default:
                break;
        }

        Move.Invoke(direction);

        AddReward(-0.01f);

        if (direction != Vector2.zero)
        {
            for (int i = 0; i < balls.Count; i++)
            {
                if (balls[i].gameObject.activeSelf)
                {
                    if (Vector3.Distance(transform.position, balls[i].transform.position) <= 5)
                    {
                        direction = balls[i].linearVelocity;
                        direction.y = 0;
                        if (Vector3.Angle(direction, transform.position - balls[i].transform.position) > 5)
                        {
                            AddReward(0.01f);
                        }
                        else
                        {
                            AddReward(-0.02f);
                        }
                    }
                    else
                    {
                        AddReward(0.02f);
                    }
                }
            }
        }

        for (int i = 0; i < opponents.Count; i++)
        {
            if (opponents[i].gameObject.activeSelf)
            {
                if (Vector3.Distance(transform.position, opponents[i].transform.position) < 5)
                {
                    if (Vector3.Angle(opponents[i].transform.forward, transform.position - opponents[i].transform.position) > 5)
                    {
                        AddReward(0.01f);
                    }
                    else
                    {
                        AddReward(-0.01f);
                    }
                }
                else
                {
                    AddReward(0.01f);
                }

                //if (Vector3.Angle(transform.forward, opponents[i].transform.position - transform.position) > 5)
                //{
                //    AddReward(0.04f);
                //}
                //else
                //{
                //    AddReward(-0.01f);
                //}
            }
        }
    }

    public void reward_hit(GameObject hitter, GameObject target, float damage)
    {
        if (hitter == gameObject || hitter == shoot.get_projectile().gameObject)
        {
            if (target == gameObject)
            {
                AddReward(-5);
            }
            else
            {
                AddReward(2f);
            }
        }
        else if (target == gameObject)
        {
            AddReward(-1f);
        }
    }

    public void reward_kill(GameObject killer, GameObject target)
    {
        if (killer == gameObject || killer == shoot.get_projectile().gameObject)
        {
            if (killer == target)
            {
                AddReward(-25);
            }
            else
            {
                AddReward(10);
            }
        }
        else if (target == gameObject)
        {
            //transform.localPosition = new Vector3(Random.Range(-10, 11), -0.5f, Random.Range(-10, 11));
            AddReward(-5f);
            EndEpisode();
            gameObject.SetActive(false);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if ((layers & (1 << collision.gameObject.layer)) != 0)
        {
            AddReward(-0.1f);
        }
    }
}
