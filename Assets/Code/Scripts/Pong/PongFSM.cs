using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PongFSM : MonoBehaviour
{
    [SerializeField] protected PongInstance instance;

    [SerializeField] protected UnityEvent<Vector2> Move;

    [SerializeField] protected float tolerance;

    protected List<Rigidbody> ball;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ball = instance.GetBall();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = Vector2.zero;
        Rigidbody closest = GetClosest();

        Vector3 predicted_pos = PredictPositionAtZ(closest.transform.localPosition, closest.linearVelocity, transform.localPosition.z);

        if (predicted_pos.x > instance.GetRightBound())
        {
            Vector3 new_vel = closest.linearVelocity;
            new_vel.x *= -1;
            predicted_pos = PredictPositionAtZ(PredictPositionAtX(closest.transform.localPosition, closest.linearVelocity, 40), new_vel, transform.localPosition.z);
        }
        else if (predicted_pos.x < instance.GetLeftBound())
        {
            Vector3 new_vel = closest.linearVelocity;
            new_vel.x *= -1;
            predicted_pos = PredictPositionAtZ(PredictPositionAtX(closest.transform.localPosition, closest.linearVelocity, -40), new_vel, transform.localPosition.z);
        }

        if (predicted_pos.x > transform.localPosition.x + tolerance)
        {
            direction = Vector2.right;
        }
        else if (predicted_pos.x < transform.localPosition.x - tolerance)
        {
            direction = Vector2.left;
        }
        else
        {
            direction.x = Random.Range(-1, 2);
        }

        Move?.Invoke(direction);
    }

    protected Rigidbody GetClosest()
    {
        Rigidbody closest = null;
        for (int i = 0; i < ball.Count; i++)
        {
            if (closest == null)
            {
                closest = ball[i];
            }
            else if (Vector3.Dot(ball[i].linearVelocity, transform.position - ball[i].transform.position) > 0.5f)
            {
                if (ZDistance(closest.transform) > ZDistance(ball[i].transform))
                {
                    closest = ball[i];
                }
            }
        }
        return closest;
    }

    protected float ZDistance(Transform other)
    {
        float distance = transform.position.z - other.position.z;
        if (distance < 0)
        {
            distance *= -1;
        }
        return distance;
    }

    protected Vector3 PredictPositionAtZ(Vector3 other, Vector3 velocity, float expected_z)
    {
        Vector3 temp = Vector3.zero;

        float time_taken = (expected_z - other.z) / velocity.z;

        temp.x = other.x + velocity.x * time_taken;
        temp.y = other.y;
        temp.z = expected_z;

        return temp;
    }

    protected Vector3 PredictPositionAtX(Vector3 other, Vector3 velocity, float expected_x)
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
