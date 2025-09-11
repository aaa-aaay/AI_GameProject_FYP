using System.Collections.Generic;
using UnityEngine;

public class BilliardSingleton : MonoBehaviour
{
    [SerializeField] private float min_magnitude;
    [SerializeField] private float max_force;
    [SerializeField] private List<GameObject> players;

    public static BilliardSingleton instance;

    private BallData cue_ball;
    private List<BallData> balls;

    private bool ball_shot;
    private bool scored;
    private bool penalty;
    private int current_player;
    private int scored_balls;

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
            scored_balls = 0;

            EventHandler.Scored += when_scored;
            EventHandler.OutOfBounds += when_out_of_bounds;
            EventHandler.EndScenario += restart_game;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        EventHandler.InvokeStartTurn(players[current_player], false);
    }

    private void OnDestroy()
    {
        EventHandler.Scored -= when_scored;
        EventHandler.OutOfBounds -= when_out_of_bounds;
        EventHandler.EndScenario -= restart_game;
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

    public BallData get_ball_data(GameObject game_object)
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
    void FixedUpdate()
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
                print("Turn over");
                next_turn();
            }
        }
    }

    public BallData get_cue_ball()
    {
        return cue_ball;
    }

    public List<BallData> get_balls()
    {
        return balls;
    }

    public void shoot_ball(Vector2 direction, float force_percentage)
    {
        direction.Normalize();
        Rigidbody temp = cue_ball.get_rigidbody();
        temp.isKinematic = false;
        temp.AddForce(new Vector3(direction.x, 0, direction.y) * force_percentage * max_force, ForceMode.Impulse);
        ball_shot = true;
    }

    public void shoot_ball(Vector3 direction, float force_percentage)
    {
        direction.y = 0;
        direction.Normalize();
        Rigidbody temp = cue_ball.get_rigidbody();
        temp.isKinematic = false;
        temp.AddForce(direction * force_percentage * max_force, ForceMode.Impulse);
        ball_shot = true;
        print("Shot ball");
    }

    public void set_cue_ball_position(Vector2 position)
    {
        cue_ball.get_ball().transform.localPosition = new Vector3(position.x, 0, position.y);
    }

    public void set_cue_ball_position(Vector3 position)
    {
        position.y = 0;
        cue_ball.get_ball().transform.localPosition = position;
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

        penalty = false;
        ball_shot = false;
        scored = false;
        print("Turn over");
    }

    public void when_scored(GameObject game_object)
    {
        if (is_cue_ball(game_object))
        {
            penalty = true;
            EventHandler.InvokePunish(players[current_player], -5f);
        }
        else if (is_ball(game_object))
        {
            scored = true;
            EventHandler.InvokePunish(players[current_player], 5f);

            scored_balls++;
            print("Score is " + scored_balls);
            if (scored_balls >= balls.Count)
            {
                EventHandler.InvokeEndScenario();
            }
        }
    }

    public void when_out_of_bounds(GameObject game_object)
    {
        if (is_cue_ball(game_object) || is_ball(game_object))
        {
            penalty = true;
            EventHandler.InvokePunish(players[current_player], -5f);
        }
    }

    public bool is_cue_ball(GameObject game_object)
    {
        return cue_ball.get_ball() == game_object;
    }

    public bool is_ball(GameObject game_object)
    {
        return get_ball_data(game_object).get_rigidbody() != null;
    }

    public void restart_game()
    {
        ball_shot = false;
        print("Game restarted");
        scored = false;
        penalty = false;
        current_player = 0;
        scored_balls = 0;
        EventHandler.InvokeStartTurn(players[current_player], false);
    }

    public void reward_player(float value)
    {
        EventHandler.InvokePunish(players[current_player], value);
    }
}
