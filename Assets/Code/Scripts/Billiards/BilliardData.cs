using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BilliardData
{
    [SerializeField] private int score;
    [SerializeField] private int current_player;
    [SerializeField] private bool shot_ball;
    [SerializeField] private bool penalty;
    [SerializeField] private bool scored;
    [SerializeField] private float min_speed;

    [SerializeField] private Rigidbody cue_ball;
    [SerializeField] private List<Rigidbody> balls;
    [SerializeField] private List<GameObject> players;
    [SerializeField] private List<GameObject> holes;

    public void start()
    {
        score = 0;
        current_player = 0;
        shot_ball = false;
        penalty = false;
        scored = false;

        EventHandler.Scored += when_scored;
        EventHandler.OutOfBounds += when_out_of_bounds;

        EventHandler.InvokeStartTurn(players[current_player], false);
    }

    public void on_destroy()
    {
        EventHandler.Scored += when_scored;
        EventHandler.OutOfBounds += when_out_of_bounds;
    }

    public bool check_win()
    {
        return score == balls.Count;
    }

    public bool is_player(GameObject game_object)
    {
        return players.Contains(game_object);
    }

    public bool is_cue_ball(GameObject game_object)
    {
        return cue_ball.gameObject == game_object;
    }

    public Rigidbody get_cue_ball()
    {
        return cue_ball;
    }

    public void shoot_ball(float x, float z)
    {
        shoot_ball(new Vector3(x, 0, z));
    }

    public void shoot_ball(Vector3 force)
    {
        if (!shot_ball)
        {
            //shot_ball = true;
            //force.y = 0;

            //cue_ball.isKinematic = false;
            //cue_ball.AddForce(force, ForceMode.Impulse);
        }

        shot_ball = true;
        force.y = 0;

        cue_ball.isKinematic = false;
        cue_ball.AddForce(force, ForceMode.Impulse);
    }

    public bool is_ball(GameObject game_object)
    {
        for (int i = 0; i < balls.Count; i++)
        {
            if (balls[i].gameObject == game_object)
            {
                return true;
            }
        }
        return false;
    }

    public Rigidbody get_ball(GameObject game_object)
    {
        if (game_object == cue_ball.gameObject)
        {
            return cue_ball;
        }    
        for (int i = 0; i < balls.Count; i++)
        {
            if (balls[i].gameObject == game_object)
            {
                return balls[i];
            }
        }
        return null;
    }

    public List<Rigidbody> get_balls()
    {
        return balls;
    }

    public bool in_game(GameObject game_object)
    {
        if (is_cue_ball(game_object))
        {
            return true;
        }
        else if (is_player(game_object))
        {
            return true;
        }
        else
        {
            if (is_ball(game_object))
            {
                return true;
            }
        }
        return false;
    }

    public void fixed_update()
    {
        if (shot_ball)
        {
            check_deceleration();
        }
        if (check_alt_win())
        {
            restart_game();
        }    
    }

    private void check_deceleration()
    {
        bool turn_over = true;
        if (!cue_ball.isKinematic)
        {
            if (cue_ball.linearVelocity.magnitude <= min_speed)
            {
                cue_ball.linearVelocity = Vector3.zero;
            }
            else
            {
                turn_over = false;
            }
        }
        for (int i = 0; i < balls.Count; i++)
        {
            if (!balls[i].isKinematic)
            {
                if (balls[i].linearVelocity.magnitude <= min_speed)
                {
                    balls[i].linearVelocity = Vector3.zero;
                }
                else
                {
                    turn_over = false;
                }
            }
        }
        if (turn_over)
        {
            next_turn();
        }
    }

    private void next_turn()
    {
        //Debug.Log(players[current_player].name + " turn over");
        if (!penalty)
        {
            if (scored)
            {
                shot_ball = false;
                penalty = false;
                scored = false;

                EventHandler.InvokeStartTurn(players[current_player], false);
            }
            else
            {
                current_player++;

                if (current_player >= players.Count)
                {
                    current_player = 0;
                }

                shot_ball = false;
                penalty = false;
                scored = false;

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

            shot_ball = false;
            penalty = false;
            scored = false;

            EventHandler.InvokeStartTurn(players[current_player], true);
        }
    }

    public void when_scored(GameObject game_object)
    {
        if (in_game(game_object))
        {
            Debug.Log(game_object.name + " was scored");
            if (is_cue_ball(game_object))
            {
                penalty = true;
                EventHandler.InvokePunish(players[current_player], -5f);
            }
            else if (is_ball(game_object))
            {
                scored = true;
                EventHandler.InvokePunish(players[current_player], 5f);

                score++;
                if (check_win())
                {
                    restart_game();
                }
            }
        }
    }

    public void when_out_of_bounds(GameObject game_object)
    {
        if (in_game(game_object))
        {
            if (is_cue_ball(game_object))
            {
                penalty = true;
                EventHandler.InvokePunish(players[current_player], -5f);
            }
            else if (is_ball(game_object))
            {
                penalty = true;
                EventHandler.InvokePunish(players[current_player], -1f);
            }
        }
    }

    public void reward_player(float reward)
    {
        EventHandler.InvokePunish(players[current_player], reward);
    }

    private void restart_game()
    {
        //Debug.Log("Won game");
        
        EventHandler.RestartGame(cue_ball.gameObject);
        for (int i = 0; i < players.Count; i++)
        {
            EventHandler.InvokeRestartGame(players[i]);
        }
        for (int i = 0; i < balls.Count; i++)
        {
            EventHandler.InvokeRestartGame(balls[i].gameObject);
        }
        score = 0;

        EventHandler.InvokeStartTurn(players[current_player], false);
    }

    public int get_score()
    {
        return score;
    }

    public bool check_alt_win()
    {
        for (int i = 0; i < balls.Count; i++)
        {
            if (balls[i].transform.position.y >= -10 && balls[i].isKinematic)
            {
                continue;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public List<GameObject> get_holes()
    {
        return holes;
    }
}
