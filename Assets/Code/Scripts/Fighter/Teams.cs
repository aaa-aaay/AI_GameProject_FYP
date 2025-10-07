using System;
using UnityEngine;

[Serializable]
public class TeamData
{
    [SerializeField] private GameObject player;
    [SerializeField] private Teams team;
    [SerializeField] private Damageable damageable;

    public TeamData(GameObject new_player, Teams new_team, Damageable new_damageable)
    {
        player = new_player;
        team = new_team;
        damageable = new_damageable;
    }

    public GameObject get_player()
    {
        return player;
    }

    public Teams get_team()
    {
        return team;
    }

    public void set_team(Teams new_team)
    {
        team = new_team;
    }

    public Damageable get_damageable()
    {
        return damageable;
    }
}

public enum Teams
{
    Player = 0,
    Enemy,
    None
}
