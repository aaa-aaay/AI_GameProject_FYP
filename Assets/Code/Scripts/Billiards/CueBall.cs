using System.Collections.Generic;
using UnityEngine;

public class CueBall : Ball
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BilliardSingleton.instance.set_cue_ball(gameObject, GetComponent<Rigidbody>());

        start_position = transform.localPosition;
        last_table_position = Vector3.zero;
        is_scored = false;

        EventHandler.Scored += out_of_bounds;
        EventHandler.OutOfBounds += out_of_bounds;
        EventHandler.EndScenario += restart;
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f, raycast_layers))
        {
            last_table_position = hit.point;
            last_table_position.y = 0;
        }

        BallData ball = BilliardSingleton.instance.get_cue_ball();

        if (ball.get_rigidbody().linearVelocity != Vector3.zero)
        {
            List<BallData> temp = BilliardSingleton.instance.get_balls();
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].get_ball().transform.position.y >= 0)
                {
                    if (Vector3.Angle(ball.get_rigidbody().linearVelocity, temp[i].get_ball().transform.position - transform.position) < 5)
                    {
                        BilliardSingleton.instance.reward_player(0.1f);
                        break;
                    }
                }
            }
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (BilliardSingleton.instance.is_cue_ball(collision.gameObject) || BilliardSingleton.instance.is_ball(collision.gameObject))
        {
            BilliardSingleton.instance.reward_player(0.01f);
        }
        else
        {
            check_scored(collision.gameObject);
        }
    }

    public override void check_scored(GameObject collided)
    {
        if (collided == scoring_area)
        {
            EventHandler.InvokeScored(gameObject);
        }
        else if (collided == penalty_area)
        {
            EventHandler.InvokeOutOfBounds(gameObject);
        }
    }

    public override void out_of_bounds(GameObject game_object)
    {
        if (gameObject == game_object)
        {
            transform.localPosition = last_table_position;
            Rigidbody temp = BilliardSingleton.instance.get_cue_ball().get_rigidbody();
            temp.linearVelocity = Vector3.zero;
            temp.isKinematic = false;
        }
    }

    public override void restart()
    {
        transform.localPosition = start_position;
        Rigidbody temp = BilliardSingleton.instance.get_cue_ball().get_rigidbody();
        temp.isKinematic = false;
        temp.linearVelocity = Vector3.zero;
        is_scored = false;
    }
}
