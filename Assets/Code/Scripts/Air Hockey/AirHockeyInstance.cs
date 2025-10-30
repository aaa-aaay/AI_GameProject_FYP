using Grpc.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Bound
{
    [SerializeField] private float upper_bound;
    [SerializeField] private float lower_bound;
    [SerializeField] private float right_bound;
    [SerializeField] private float left_bound;

    public bool WithinBounds(Transform transform)
    {
        if (transform.localPosition.x < right_bound)
        {
            if (transform.localPosition.x > left_bound)
            {
                if (transform.localPosition.z < upper_bound)
                {
                    if (transform.localPosition.z > lower_bound)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void BindPosition(Transform transform)
    {
        if (transform.localPosition.x + transform.localScale.x * 0.5f > right_bound)
        {
            Vector3 temp = transform.localPosition;
            temp.x = right_bound - (transform.localScale.x * 0.5f);
            transform.localPosition = temp;
        }
        else if (transform.localPosition.x - transform.localScale.x * 0.5f < left_bound)
        {
            Vector3 temp = transform.localPosition;
            temp.x = left_bound + (transform.localScale.x * 0.5f);
            transform.localPosition = temp;
        }
        if (transform.localPosition.z + transform.localScale.z * 0.5f > upper_bound)
        {
            Vector3 temp = transform.localPosition;
            temp.z = upper_bound - (transform.localScale.z * 0.5f);
            transform.localPosition = temp;
        }
        else if (transform.localPosition.z - transform.localScale.z * 0.5f < lower_bound)
        {
            Vector3 temp = transform.localPosition;
            temp.z = lower_bound + (transform.localScale.z * 0.5f);
            transform.localPosition = temp;
        }
    }
}

public class AirHockeyInstance : MonoBehaviour
{
    [SerializeField] private UnityEvent<int, int> ScoreChanged;

    [SerializeField] private List<Rigidbody> pucks;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject opponent;

    [SerializeField] private Bound player_scoring_bound;
    [SerializeField] private Bound opponent_scoring_bound;

    [SerializeField] private float game_time;
    [SerializeField] private int win_score;

    private int player_points;
    private int opponent_points;
    private float time_passed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Restart();
    }

    private void Update()
    {
        for (int i = 0; i < pucks.Count; i++)
        {
            if (player_scoring_bound.WithinBounds(pucks[i].transform))
            {
                opponent_points++;
                ScoreChanged?.Invoke(player_points, opponent_points);
                EventHolder.InvokeOnRestart(pucks[i].gameObject);
                if (opponent_points >= win_score)
                {
                    EndGame();
                }
            }
            else if (opponent_scoring_bound.WithinBounds(pucks[i].transform))
            {
                player_points++;
                ScoreChanged?.Invoke(player_points, opponent_points);
                EventHolder.InvokeOnRestart(pucks[i].gameObject);
                if (player_points >= win_score)
                {
                    EndGame();
                }
            }
        }
        time_passed += Time.deltaTime;
        if (time_passed > game_time)
        {
            EndGame();
        }
    }

    private void Restart()
    {
        player_points = 0;
        opponent_points = 0;
        time_passed = 0;

        ScoreChanged?.Invoke(player_points, opponent_points);
    }

    private void EndGame()
    {
        if (player_points == opponent_points)
        {
            EventHolder.InvokeOnDraw(player);
            EventHolder.InvokeOnDraw(opponent);
        }
        else if (opponent_points > player_points)
        {
            EventHolder.InvokeOnLose(player);
            EventHolder.InvokeOnWin(opponent);
        }
        else
        {
            EventHolder.InvokeOnWin(player);
            EventHolder.InvokeOnLose(opponent);
        }

        for (int i = 0; i < pucks.Count; i++)
        {
            EventHolder.InvokeOnRestart(pucks[i].gameObject);
        }

        Restart();
    }

    public List<Rigidbody> GetPuck()
    {
        return pucks;
    }

    public float GetTimePassed()
    {
        return time_passed;
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    public GameObject GetOpponent()
    {
        return opponent;
    }

    public int GetPlayerPoints()
    {
        return player_points;
    }

    public int GetOpponentPoints()
    {
        return opponent_points;
    }
}
