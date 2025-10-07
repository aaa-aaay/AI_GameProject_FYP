using System.Collections.Generic;
using UnityEngine;

public class TeamSingleton : MonoBehaviour
{
    public static TeamSingleton instance;

    [SerializeField] private List<TeamData> teams;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            teams = new List<TeamData>();
            EventHandler.GotKill += change_team;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public void join_team(GameObject game_object, Teams team)
    {
        teams.Add(new TeamData(game_object, team, gameObject.GetComponent<Damageable>()));
    }

    public void change_team(GameObject game_object, Teams team, bool check_win = true)
    {
        if (team != Teams.None)
        {
            TeamData temp = get_team_data(game_object);
            if (teams.Contains(temp))
            {
                teams.Remove(temp);
                temp.set_team(team);
                teams.Add(temp);

                if (check_win)
                {
                    check_restart();
                }
            }
        }
    }

    public void change_team(GameObject killer, GameObject target)
    {
        change_team(target, get_team(killer));
    }

    public bool check_same_team(GameObject game_object, GameObject other)
    {
        return (get_team(game_object) == get_team(other));
    }

    public Teams get_team(GameObject game_object)
    {
        return get_team_data(game_object).get_team();
    }

    public Teams get_team(int index)
    {
        return teams[index].get_team();
    }

    public Damageable get_damageable(GameObject game_object)
    {
        return get_team_data(game_object).get_damageable();
    }

    public float get_health(GameObject game_object)
    {
        Damageable temp = get_damageable(game_object);
        if (temp == null) 
        {
            return 0;
        }
        return temp.get_health();
    }

    public TeamData get_team_data(GameObject game_object)
    {
        TeamData temp;
        for (int i = 0; i < teams.Count; i++)
        {
            temp = teams[i];
            if (temp.get_player() == game_object)
            {
                return temp;
            }
        }

        print("Couldn't find");
        temp = new TeamData(gameObject, Teams.None, gameObject.GetComponent<Damageable>());
        return temp;
    }

    private void check_restart()
    {
        Teams temp = get_team(0);
        for (int i = 0; i < teams.Count; i++)
        {
            if (temp != get_team(i))
            {
                return;
            }
        }
        print("Won game");
        for (int i = 0; i < teams.Count; i++)
        {
            EventHandler.InvokePunish(teams[i].get_player(), 15);
        }
        EventHandler.InvokeEndScenario();
    }
}
