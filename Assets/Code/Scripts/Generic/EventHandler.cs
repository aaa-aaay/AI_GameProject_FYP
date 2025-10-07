using System;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler
{
    public static Action EndScenario;

    public static Action<GameObject, GameObject, float> GotHit;
    public static Action<GameObject, GameObject, float> TookDamage;
    public static Action<GameObject, GameObject> GotKill;
    public static Action<GameObject, float> Punish;

    public static Action<GameObject, bool> StartTurn;
    public static Action<GameObject> OutOfBounds;
    public static Action<GameObject> Scored;
    public static Action<GameObject> RestartGame;

    public static Action<GameObject> FailedHit;

    public static void InvokeEndScenario()
    {
        if (EndScenario != null)
        {
            EndScenario.Invoke();
        }
    }

    public static void InvokeGotHit(GameObject hitter, GameObject target, float damage)
    {
        if (GotHit != null)
        {
            GotHit.Invoke(hitter, target, damage);
        }
    }

    public static void InvokeTookDamage(GameObject hitter, GameObject target, float damage)
    {
        if (TookDamage != null)
        {
            TookDamage.Invoke(hitter, target, damage);
        }
    }

    public static void InvokeGotKill(GameObject killer, GameObject target)
    {
        if (GotKill != null)
        {
            GotKill.Invoke(killer, target);
        }
    }

    public static void InvokePunish(GameObject target, float punishment)
    {
        if (Punish != null)
        {
            Punish.Invoke(target, punishment);
        }
    }

    public static void InvokeStartTurn(GameObject player, bool set_position)
    {
        if (StartTurn != null)
        {
            StartTurn.Invoke(player, set_position);
        }
    }

    public static void InvokeOutOfBounds(GameObject game_object)
    {
        if (OutOfBounds != null)
        {
            OutOfBounds.Invoke(game_object);
        }
    }

    public static void InvokeScored(GameObject game_object)
    {
        if (Scored != null)
        {
            Scored.Invoke(game_object);
        }
    }

    public static void InvokeRestartGame(GameObject game_object)
    {
        if (RestartGame != null)
        {
            RestartGame.Invoke(game_object);
        }
    }

    public static void InvokeFailedHit(GameObject game_object)
    {
        if (FailedHit != null)
        {
            FailedHit.Invoke(game_object);
        }
    }
}
