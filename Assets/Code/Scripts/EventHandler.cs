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

    public static Action<GameObject, bool> TurnStart;
    public static Action<GameObject> TurnEnd;
    public static Action<GameObject> Scored;
    public static Action<GameObject> OutOfBounds;

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

    public static void InvokeTurnStart(GameObject player, bool can_select_position)
    {
        if (TurnStart != null)
        {
            TurnStart.Invoke(player, can_select_position);
        }
    }

    public static void InvokeTurnEnd(GameObject player)
    {
        if (TurnEnd != null)
        {
            TurnEnd.Invoke(player);
        }
    }

    public static void InvokeScored(GameObject scorer)
    {
        if (Scored != null)
        {
            Scored.Invoke(scorer);
        }
    }

    public static void InvokeOutOfBounds(GameObject player)
    {
        if (OutOfBounds != null)
        {
            OutOfBounds.Invoke(player);
        }
    }
}
