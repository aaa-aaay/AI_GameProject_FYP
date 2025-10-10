using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PongInstance : MonoBehaviour
{
    [SerializeField] private List<Rigidbody> balls;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject opponent;

    [SerializeField] private UnityEvent<GameObject, GameObject> Scored;
    [SerializeField] private UnityEvent<int, int> ScoreChanged;

    [SerializeField] private float game_time;
    [SerializeField] private float left_scoring_bounds;
    [SerializeField] private float right_scoring_bounds;
    [SerializeField] private float win_score = 10;

    private int player_points;
    private int opponent_points;
    private float time_passed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Restart();
    }

    // Update is called once per frame
    void Update()
    {
        time_passed += Time.deltaTime;

        if (player.transform.localPosition.x > 37)
        {
            player.transform.localPosition = new Vector3(37, player.transform.localPosition.y, -60);
        }
        else if (player.transform.localPosition.x < -37)
        {
            player.transform.localPosition = new Vector3(-37, player.transform.localPosition.y, -60);
        }

        if (opponent.transform.localPosition.x > 37)
        {
            opponent.transform.localPosition = new Vector3(37, opponent.transform.localPosition.y, 60);
        }
        else if (opponent.transform.localPosition.x < -37)
        {
            opponent.transform.localPosition = new Vector3(-37, opponent.transform.localPosition.y, 60);
        }

        for (int i = 0; i < balls.Count; i++)
        {
            if (balls[i].transform.localPosition.z < left_scoring_bounds)
            {
                opponent_points++;
                Scored?.Invoke(opponent, balls[i].gameObject);
                ScoreChanged?.Invoke(player_points, opponent_points);
                EventHolder.InvokeOnRestart(balls[i].gameObject);

                if (opponent_points >= win_score)
                {
                    Restart();
                }
            }
            else if (balls[i].transform.localPosition.z > right_scoring_bounds)
            {
                player_points++;
                Scored?.Invoke(player, balls[i].gameObject);
                ScoreChanged?.Invoke(player_points, opponent_points);
                EventHolder.InvokeOnRestart(balls[i].gameObject);

                if (player_points >= win_score)
                {
                    Restart();
                }
            }
        }

        if (time_passed > game_time)
        {
            Restart();
        }
    }

    private void Restart()
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

        for (int i = 0; i < balls.Count; i++)
        {
            EventHolder.InvokeOnRestart(balls[i].gameObject);
        }

        player_points = 0;
        opponent_points = 0;
        time_passed = 0;

        ScoreChanged?.Invoke(player_points, opponent_points);
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

    public List<Rigidbody> GetBall()
    {
        return balls;
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
