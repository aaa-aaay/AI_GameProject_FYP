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
}
