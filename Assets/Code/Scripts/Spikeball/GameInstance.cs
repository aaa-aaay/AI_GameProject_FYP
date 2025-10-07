using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class GameInstance : MonoBehaviour
{
    [SerializeField] private List<SimpleDamageable> units;
    [SerializeField] private float game_time = 60;

    private float time_passed;

    private void Start()
    {
        EventHandler.GotKill += when_killed;
    }

    private void OnDestroy()
    {
        EventHandler.GotKill -= when_killed;
    }

    private void Update()
    {
        time_passed += Time.deltaTime;

        if (time_passed > game_time)
        {
            punish_all(-10);
            restart_game();
        }
    }

    public void when_killed(GameObject killer, GameObject target)
    {
        check_win();
    }

    private void check_win()
    {
        int alive = 0;
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].is_alive())
            {
                alive++;
            }
        }
        if (alive <= 1)
        {
            restart_game();
        }
    }

    private void restart_game()
    {
        for (int i = 0; i < units.Count; i++)
        {
            time_passed = 0;
            units[i].gameObject.SetActive(true);
            EventHandler.InvokeRestartGame(units[i].gameObject);
        }
    }

    private void punish_all(float punishment)
    {
        for (int i = 0; i < units.Count; i++)
        {
            EventHandler.InvokePunish(units[i].gameObject, punishment);
        }
    }
}
