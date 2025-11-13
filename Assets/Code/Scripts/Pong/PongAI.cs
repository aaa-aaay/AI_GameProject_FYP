using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Unity.InferenceEngine;

public class PongAI : AgentDLC
{
    [SerializeField] private PongInstance instance;

    [SerializeField] private UnityEvent<Vector2> Move;

    [SerializeField] private float offset = 2.5f;

    private Vector3 predicted_pos;

    private List<Rigidbody> balls;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        balls = instance.GetBall();

        EventHolder.OnWin += OnWin;
        EventHolder.OnLose += OnLose;
        EventHolder.OnDraw += OnDraw;
        EventHolder.OnRestart += OnRestart;
    }

    private void OnDestroy()
    {
        EventHolder.OnWin -= OnWin;
        EventHolder.OnLose -= OnLose;
        EventHolder.OnDraw -= OnDraw;
        EventHolder.OnRestart -= OnRestart;
    }

    // Update is called once per frame
    void Update()
    {
        RequestDecision();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(instance.GetPlayer().transform.localPosition);
        sensor.AddObservation(instance.GetOpponent().transform.localPosition);
        sensor.AddObservation(instance.GetPlayerPoints());
        sensor.AddObservation(instance.GetOpponentPoints());

        for (int i = 0; i < balls.Count; i++)
        {
            sensor.AddObservation(balls[i].transform.localPosition);
            sensor.AddObservation(balls[i].linearVelocity);
        }

        //sensor.AddObservation(predicted_pos);
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

        Rigidbody closest = GetClosest();

        predicted_pos = PredictPositionAtZ(closest.transform.localPosition, closest.linearVelocity, transform.localPosition.z);

        if (predicted_pos.x > instance.GetRightBound())
        {
            Vector3 new_vel = closest.linearVelocity;
            new_vel.x *= -1;
            predicted_pos = PredictPositionAtZ(PredictPositionAtX(closest.transform.localPosition, closest.linearVelocity, instance.GetRightBound()), new_vel, transform.localPosition.z);
        }
        else if (predicted_pos.x < instance.GetLeftBound())
        {
            Vector3 new_vel = closest.linearVelocity;
            new_vel.x *= -1;
            predicted_pos = PredictPositionAtZ(PredictPositionAtX(closest.transform.localPosition, closest.linearVelocity, instance.GetLeftBound()), new_vel, transform.localPosition.z);
        }

        if (predicted_pos.x < transform.localPosition.x + offset && predicted_pos.x > transform.localPosition.x - offset)
        {
            AddReward(0.25f);
        }
        if ((predicted_pos.x - transform.localPosition.x) > 0 && direction.x > 0)
        {
            AddReward(0.1f);
        }
        else if ((predicted_pos.x - transform.localPosition.x) < 0 && direction.x < 0)
        {
            AddReward(0.1f);
        }

        AddReward(-0.01f); // To prevent stalling
    }

    protected override void OnWin(GameObject player)
    {
        if (player == gameObject)
        {
            //AddReward(25);
            EventHolder.InvokeOnRestart(gameObject);
        }
    }

    protected override void OnLose(GameObject player)
    {
        if (player == gameObject)
        {
            //AddReward(-25);
            EventHolder.InvokeOnRestart(gameObject);
        }
    }

    protected override void OnDraw(GameObject player)
    {
        if (player == gameObject)
        {
            //AddReward(10);
            EventHolder.InvokeOnRestart(gameObject);
        }
    }

    protected override void OnRestart()
    {
        EndEpisode();
    }

    private void OnCollisionEnter(Collision collision)
    {
        for (int i = 0; i < balls.Count; i++)
        {
            if (collision.gameObject == balls[i].gameObject)
            {
                AddReward(2.5f);
                print("Hit back balls reward");
                break;
            }
        }
    }

    public void Scored(GameObject player, GameObject scored)
    {
        if (player == gameObject)
        {
            AddReward(5);
        }
        else
        {
            //AddReward(-3);
            AddReward(-(Vector3.Distance(transform.position, scored.transform.position) * 0.5f));
        }
    }

    private Rigidbody GetClosest()
    {
        Rigidbody closest = null;
        for (int i = 0; i < balls.Count; i++)
        {
            if (closest == null)
            {
                closest = balls[i];
            }
            else if (Vector3.Dot(balls[i].linearVelocity, transform.position - balls[i].transform.position) > 0.25f)
            {
                if (ZDistance(closest.transform) > ZDistance(balls[i].transform))
                {
                    closest = balls[i];
                }
            }
        }
        return closest;
    }

    private float ZDistance(Transform other)
    {
        float distance = transform.position.z - other.position.z;
        if (distance < 0)
        {
            distance *= -1;
        }
        return distance;
    }

    private Vector3 PredictPositionAtZ(Vector3 other, Vector3 velocity, float expected_z)
    {
        Vector3 temp = Vector3.zero;

        float time_taken = (expected_z - other.z) / velocity.z;

        temp.x = other.x + velocity.x * time_taken;
        temp.y = other.y;
        temp.z = expected_z;

        return temp;
    }

    private Vector3 PredictPositionAtX(Vector3 other, Vector3 velocity, float expected_x)
    {
        Vector3 temp = Vector3.zero;

        velocity = velocity.normalized;

        float time_taken = (expected_x - other.x) / velocity.x;

        temp.x = expected_x;
        temp.y = other.y;
        temp.z = other.z + velocity.z * time_taken;

        return temp;
    }
}
