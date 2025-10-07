using UnityEngine;
using System.Collections.Generic;

public class CueBall : Ball
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        start_position = transform.position;
        last_position = transform.position;
        rigidbody = GetComponent<Rigidbody>();

        EventHandler.Scored += when_out_of_bounds;
        EventHandler.OutOfBounds += when_out_of_bounds;
        EventHandler.RestartGame += restart_game;
    }

    protected override void OnCollisionStay(Collision collision)
    {
        if (!scored)
        {
            if (scoring_area.Contains(collision.gameObject))
            {
                EventHandler.InvokeScored(gameObject);
            }
            else if (penalty_area.Contains(collision.gameObject))
            {
                EventHandler.InvokeOutOfBounds(gameObject);
            }
            else
            {
                List<Rigidbody> balls = BilliardSingleton.instance.get_balls(gameObject);
                for (int i = 0; i < balls.Count; i++)
                {
                    if (balls[i].gameObject == collision.gameObject)
                    {
                        BilliardSingleton.instance.reward_player(gameObject, 0.1f);
                    }
                }
            }
        }
    }

    private void OnDestroy()
    {
        EventHandler.Scored -= when_out_of_bounds;
        EventHandler.OutOfBounds -= when_out_of_bounds;
        EventHandler.RestartGame -= restart_game;
    }

    public override void when_out_of_bounds(GameObject game_object)
    {
        if (gameObject == game_object)
        {
            transform.position = last_position;
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.isKinematic = true;
        }
    }
}
