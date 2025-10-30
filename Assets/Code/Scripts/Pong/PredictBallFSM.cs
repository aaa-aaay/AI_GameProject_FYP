using UnityEngine;

public class PredictBallFSM : PongFSM
{
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

        if (predicted_pos.x > 40)
        {
            Vector3 new_vel = closest.linearVelocity;
            new_vel.x *= -1;
            predicted_pos = PredictPositionAtZ(PredictPositionAtX(closest.transform.localPosition, closest.linearVelocity, 40), new_vel, transform.localPosition.z);
        }
        else if (predicted_pos.x < -40)
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
}
