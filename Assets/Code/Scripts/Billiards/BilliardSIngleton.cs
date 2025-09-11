using System.Collections.Generic;
using UnityEngine;

public class BilliardSIngleton : MonoBehaviour
{
    [SerializeField] private float min_magnitude;
    [SerializeField] private float max_force;
    [SerializeField] private List<GameObject> players;

    public static BilliardSIngleton instance;

    private BallData cue_ball;
    private List<BallData> balls;

    private bool ball_shot;
    private bool scored;
    private bool penalty;
    private int current_player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            balls = new List<BallData>();

            ball_shot = false;
            scored = false;
            penalty = false;
            current_player = 0;
        }
        else
        {
            Destroy(this);
        }
    }

    public void set_cue_ball(GameObject new_cue_ball, Rigidbody rigidbody)
    {
        cue_ball = new BallData(new_cue_ball, rigidbody);
    }

    public void set_new_ball(GameObject new_ball, Rigidbody rigidbody)
    {
        BallData temp = get_ball_data(new_ball);
        if (temp.get_rigidbody() == null)
        {
            temp.set_rigidbody(rigidbody);
            balls.Add(temp);
        }
    }

    private BallData get_ball_data(GameObject game_object)
    {
        BallData temp;
        for (int i = 0; i < balls.Count; i++)
        {
            temp = balls[i];
            if (temp.get_ball() == game_object)
            {
                return temp;
            }
        }
        temp = new BallData(game_object, null);
        return temp;
    }

    // Update is called once per frame
    void Update()
    {
        if (ball_shot)
        {
            Rigidbody temp;
            bool finished = true;

            temp = cue_ball.get_rigidbody();

            if (temp.linearVelocity.magnitude <= min_magnitude)
            {
                temp.linearVelocity = Vector3.zero;
            }
            else
            {
                finished = false;
            }

            for (int i = 0; i < balls.Count; i++)
            {
                temp = balls[i].get_rigidbody();

                if (temp.linearVelocity.magnitude <= min_magnitude)
                {
                    temp.linearVelocity = Vector3.zero;
                }
                else
                {
                    finished = false;
                }
            }

            if (finished && ball_shot)
            {
                next_turn();
            }
        }
    }

    public BallData get_cue_ball()
    {
        return cue_ball;
    }

    public List<BallData> get_ball_data()
    {
        return balls;
    }

    public void shoot_ball(Vector2 direction, float force_percentage)
    {
        cue_ball.get_rigidbody().AddForce(new Vector3(direction.x, 0, direction.y) * force_percentage * max_force);
        ball_shot = true;
    }

    public void shoot_ball(Vector3 direction, float force_percentage)
    {
        cue_ball.get_rigidbody().AddForce(direction * force_percentage * max_force);
        ball_shot = true;
    }

    public void set_cue_ball_position(Vector2 position)
    {
        cue_ball.get_ball().transform.position = new Vector3(position.x, 0, position.y);
    }

    private void next_turn()
    {
        if (!penalty)
        {
            if (scored)
            {
                EventHandler.InvokeStartTurn(players[current_player], false);
            }
            else
            {
                current_player++;

                if (current_player >= players.Count)
                {
                    current_player = 0;
                }

                EventHandler.InvokeStartTurn(players[current_player], false);
            }
        }
        else
        {
            current_player++;

            if (current_player >= players.Count)
            {
                current_player = 0;
            }

            EventHandler.InvokeStartTurn(players[current_player], true);
        }
    }
}
