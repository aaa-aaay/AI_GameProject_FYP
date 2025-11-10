using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PongInstance : MonoBehaviour
{
    [SerializeField] private List<Rigidbody> balls;
    [SerializeField] private List<GameObject> power_ups;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject opponent;

    [SerializeField] private UnityEvent<GameObject, GameObject> Scored;
    [SerializeField] private UnityEvent<int, int> ScoreChanged;

    [SerializeField] private float game_time;
    [SerializeField] private float lower_score_bounds;
    [SerializeField] private float upper_score_bounds;
    [SerializeField] private float left_bound;
    [SerializeField] private float right_bound;
    [SerializeField] private float win_score = 10;

    [SerializeField] private float min_power_up_time;
    [SerializeField] private float max_power_up_time;

    private int player_points;
    private int opponent_points;
    private float time_passed;
    private float power_up_time;
    private float power_time_passed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Restart();
    }

    // Update is called once per frame
    void Update()
    {
        time_passed += Time.deltaTime;
        power_time_passed += Time.deltaTime;

        if (PongUI.instance.GetActiveCount() < 2)
        {
            if (power_time_passed > power_up_time)
            {
                Instantiate(power_ups[Random.Range(0, power_ups.Count)]).transform.position = new Vector3(Random.Range(left_bound, right_bound) * 0.9f, 1.5f, Random.Range(lower_score_bounds, upper_score_bounds) * 0.6f);

                PongUI.instance.AddCount();
                power_time_passed = 0;
                power_up_time = Random.Range(min_power_up_time, max_power_up_time);
            }
        }

        if (player.transform.localPosition.x + 2.35f > right_bound)
        {
            player.transform.localPosition = new Vector3(right_bound - 2.35f, player.transform.localPosition.y, player.transform.localPosition.z);
        }
        else if (player.transform.localPosition.x - 2.35f < left_bound)
        {
            player.transform.localPosition = new Vector3(left_bound + 2.35f, player.transform.localPosition.y, player.transform.localPosition.z);
        }

        if (opponent.transform.localPosition.x + 2.35f > right_bound)
        {
            opponent.transform.localPosition = new Vector3(right_bound - 2.35f, opponent.transform.localPosition.y, opponent.transform.localPosition.z);
        }
        else if (opponent.transform.localPosition.x - 2.35f < left_bound)
        {
            opponent.transform.localPosition = new Vector3(left_bound + 2.35f, opponent.transform.localPosition.y, opponent.transform.localPosition.z);
        }

        for (int i = 0; i < balls.Count; i++)
        {
            if (balls[i].transform.localPosition.z < lower_score_bounds * 0.8f)
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
            else if (balls[i].transform.localPosition.z > upper_score_bounds * 0.8f)
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

        MiniGameOverHandler gameOverHandler = GetComponent<MiniGameOverHandler>();

        if (player_points == opponent_points)
        {
            EventHolder.InvokeOnDraw(player);
            EventHolder.InvokeOnDraw(opponent);


        }
        else if (opponent_points > player_points)
        {
            EventHolder.InvokeOnLose(player);
            EventHolder.InvokeOnWin(opponent);

            gameOverHandler.HandleGameOver(false);
        }
        else
        {
            gameOverHandler.HandleGameOver(true,3,3);
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
        power_time_passed = 0;

        power_up_time = Random.Range(min_power_up_time, max_power_up_time);

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

    public float GetLeftBound()
    {
        return left_bound;
    }

    public float GetRightBound()
    {
        return right_bound;
    }
}
