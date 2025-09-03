using System;
using UnityEngine;

public class EventHandler
{
    public static Action<GameObject, GameObject, float> GotHit;
    public static Action<GameObject, GameObject> GotKill;

    public static void InvokeGotHit(GameObject hitter, GameObject target, float damage)
    {
        if (GotHit != null)
        {
            GotHit.Invoke(hitter, target, damage);
        }
    }

    public static void InvokeGotKill(GameObject killer, GameObject target)
    {
        if (GotKill != null)
        {
            GotKill.Invoke(killer, target);
        }
    }
}
