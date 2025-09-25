using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class DodgeballData
{
    [SerializeField] private GameObject player;

    private Damageable damageable;
    private ShootInterface shoot;

    public void InitializeData()
    {
        damageable = player.GetComponent<Damageable>();
        shoot = player.GetComponent<ShootInterface>();
    }

    public Damageable GetDamageable()
    {
        return damageable;
    }

    public ShootInterface GetShoot()
    {
        return shoot;
    }

    public GameObject GetPlayer()
    {
        return player;
    }
}

public class DodgeballInstance : MonoBehaviour
{
    [SerializeField] private List<GameObject> walls;
    [SerializeField] private List<DodgeballData> players;
    [SerializeField] private List<DodgeballData> opponents;
    [SerializeField] private List<Dodgeball> balls;

    [SerializeField] private float game_time = 60;

    [SerializeField] private float player_left_boundary;
    [SerializeField] private float player_right_boundary;
    [SerializeField] private float player_top_boundary;
    [SerializeField] private float player_bottom_boundary;

    [SerializeField] private float opponent_left_boundary;
    [SerializeField] private float opponent_right_boundary;
    [SerializeField] private float opponent_top_boundary;
    [SerializeField] private float opponent_bottom_boundary;

    private float time_passed;

    private void Update()
    {
        time_passed += Time.deltaTime;
        if (time_passed > game_time)
        {
            TimeUp();
        }
    }

    private void FixedUpdate()
    {
        CheckWin();
        WithinBoundary();
    }

    public List<Damageable> GetDamageablePlayers()
    {
        List<Damageable> temp = new List<Damageable>();
        for (int i = 0; i < players.Count; i++)
        {
            temp.Add(players[i].GetDamageable());
        }
        return temp;
    }

    public List<Damageable> GetDamageableOpponents()
    {
        List<Damageable> temp = new List<Damageable>();
        for (int i = 0; i < opponents.Count; i++)
        {
            temp.Add(opponents[i].GetDamageable());
        }
        return temp;
    }

    public List<Dodgeball> GetDodgeballs()
    {
        List<Dodgeball> temp = new List<Dodgeball>();
        for (int i = 0; i < balls.Count; i++)
        {
            temp.Add(balls[i]);
        }
        return temp;
    }

    protected void CheckWin()
    {
        int alive_players = AliveMembers(GetDamageablePlayers());
        int alive_opponents = AliveMembers(GetDamageableOpponents());
        if (alive_players <= 0 || alive_opponents <= 0)
        {
            if (alive_players == alive_opponents)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    players[i].GetPlayer()  .SetActive(true);
                    EventHolder.InvokeOnLose(players[i].GetPlayer());
                    EventHolder.InvokeOnRestart(players[i].GetPlayer());
                }
                for (int i = 0; i < opponents.Count; i++)
                {
                    opponents[i].GetPlayer().SetActive(true);
                    EventHolder.InvokeOnLose(opponents[i].GetPlayer());
                    EventHolder.InvokeOnRestart(opponents[i].GetPlayer());
                }
            }
            else if (alive_players < alive_opponents)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    players[i].GetPlayer().SetActive(true);
                    EventHolder.InvokeOnLose(players[i].GetPlayer());
                    EventHolder.InvokeOnRestart(players[i].GetPlayer());
                }
                for (int i = 0; i < opponents.Count; i++)
                {
                    opponents[i].GetPlayer().SetActive(true);
                    EventHolder.InvokeOnWin(opponents[i].GetPlayer());
                    EventHolder.InvokeOnRestart(opponents[i].GetPlayer());
                }
            }
            else
            {
                for (int i = 0; i < players.Count; i++)
                {
                    players[i].GetPlayer().SetActive(true);
                    EventHolder.InvokeOnWin(players[i].GetPlayer());
                    EventHolder.InvokeOnRestart(players[i].GetPlayer());
                }
                for (int i = 0; i < opponents.Count; i++)
                {
                    opponents[i].GetPlayer().SetActive(true);
                    EventHolder.InvokeOnLose(opponents[i].GetPlayer());
                    EventHolder.InvokeOnRestart(opponents[i].GetPlayer());
                }
            }

            for (int i = 0; i < balls.Count; i++)
            {
                EventHolder.InvokeOnRestart(balls[i].gameObject);
            }
        }
    }

    private void TimeUp()
    {
        int alive_players = AliveMembers(GetDamageablePlayers());
        int alive_opponents = AliveMembers(GetDamageableOpponents());

        if (alive_players == alive_opponents)
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].GetPlayer().SetActive(true);
                EventHolder.InvokeOnLose(players[i].GetPlayer());
                EventHolder.InvokeOnRestart(players[i].GetPlayer());
            }
            for (int i = 0; i < opponents.Count; i++)
            {
                opponents[i].GetPlayer().SetActive(true);
                EventHolder.InvokeOnLose(opponents[i].GetPlayer());
                EventHolder.InvokeOnRestart(opponents[i].GetPlayer());
            }
        }
        else if (alive_players < alive_opponents)
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].GetPlayer().SetActive(true);
                EventHolder.InvokeOnLose(players[i].GetPlayer());
                EventHolder.InvokeOnRestart(players[i].GetPlayer());
            }
            for (int i = 0; i < opponents.Count; i++)
            {
                opponents[i].GetPlayer().SetActive(true);
                EventHolder.InvokeOnWin(opponents[i].GetPlayer());
                EventHolder.InvokeOnRestart(opponents[i].GetPlayer());
            }
        }
        else
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].GetPlayer().SetActive(true);
                EventHolder.InvokeOnWin(players[i].GetPlayer());
                EventHolder.InvokeOnRestart(players[i].GetPlayer());
            }
            for (int i = 0; i < opponents.Count; i++)
            {
                opponents[i].GetPlayer().SetActive(true);
                EventHolder.InvokeOnLose(opponents[i].GetPlayer());
                EventHolder.InvokeOnRestart(opponents[i].GetPlayer());
            }
        }

        for (int i = 0; i < balls.Count; i++)
        {
            EventHolder.InvokeOnRestart(balls[i].gameObject);
        }

        time_passed = 0;    
    }

    private void WithinBoundary()
    {
        Vector3 temp;

        for (int i = 0; i < players.Count; i++)
        {
            temp = players[i].GetDamageable().transform.localPosition;
            
            if (temp.x < player_left_boundary)
            {
                temp.x = player_left_boundary;
            }
            else if (temp.x > player_right_boundary)
            {
                temp.x = player_right_boundary;
            }

            if (temp.z > player_top_boundary)
            {
                temp.z = player_top_boundary;
            }
            else if (temp.z < player_bottom_boundary) 
            {
                temp.z = player_top_boundary;
            }

            players[i].GetPlayer().transform.localPosition = temp;
        }

        for (int i = 0; i < opponents.Count; i++)
        {
            temp = opponents[i].GetPlayer().transform.localPosition;

            if (temp.x < opponent_left_boundary)
            {
                 temp.x = opponent_left_boundary;
            }
            else if (temp.x > opponent_right_boundary)
            {
                temp.x = opponent_right_boundary;
            }

            if (temp.z > opponent_top_boundary)
            {
                temp.z = opponent_top_boundary;
            }
            else if (temp.z <  opponent_bottom_boundary)
            {
                temp.z = opponent_bottom_boundary;
            }

            opponents[i].GetDamageable().transform.localPosition = temp;
        }
    }

    public List<GameObject> GetWalls()
    {
        return walls;
    }

    public float GetGameTime()
    {
        return game_time;
    }

    public float GetTimePassed()
    {
        return time_passed;
    }

    private int AliveMembers(List<Damageable> team)
    {
        int alive = 0;
        for (int i = 0; i < team.Count; i++)
        {
            if (team[i].IsAlive())
            {
                alive++;
            }
        }
        return alive;
    }

    public bool CheckSameTeam(Damageable player, Damageable compared)
    {
        if (IsPlayer(player) && IsPlayer(compared))
        {
            return true;
        }
        else if (IsEnemy(player) && IsEnemy(compared))
        {
            return true;
        }
        return false;
    }

    public bool IsPlayer(Damageable player)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].GetDamageable() == player)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsEnemy(Damageable player)
    {
        for (int i = 0; i < opponents.Count; i++)
        {
            if (opponents[i].GetDamageable() == player)
            {
                return true;
            }
        }
        return false;
    }
}
