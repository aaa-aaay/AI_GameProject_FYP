using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> players;

    private GameObject player;
    private bool scored;
    private bool out_of_bounds;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        EventHandler.TurnEnd += handle_next;
        EventHandler.OutOfBounds += when_out_of_bounds;
        EventHandler.Scored += when_scoring;

        player = players[0];

        scored = false;
        out_of_bounds = false;
    }

    private void Start()
    {
        EventHandler.InvokeTurnStart(player, false);
    }

    public void when_scoring(GameObject  scorer)
    {
        if (player == scorer)
        {
            scored = true;
        }
    }

    public void when_out_of_bounds(GameObject other_player)
    {
        if (player == other_player)
        {
            out_of_bounds = true;
        }
    }    

    public void handle_next(GameObject other_player)
    {
        if (player == other_player)
        {
            if (out_of_bounds)
            {
                int temp = players.IndexOf(other_player);

                out_of_bounds = false;
                scored = false;

                temp++;

                if (temp >= players.Count)
                {
                    temp = 0;
                }

                player = players[temp];

                EventHandler.InvokeTurnStart(player, true);
            }
            else if (scored)
            {
                scored = false;
                EventHandler.InvokeTurnStart(player, false);
            }
            else
            {
                int temp = players.IndexOf(other_player);

                out_of_bounds = false;
                scored = false;

                temp++;

                if (temp >= players.Count)
                {
                    temp = 0;
                }

                player = players[temp];

                EventHandler.InvokeTurnStart(player, false);
            }
        }
        else
        {
            print(player.name + " wtf " + other_player.name);
        }
    }
}