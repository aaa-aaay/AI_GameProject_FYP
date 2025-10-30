using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventHolder
{
    public static Action<GameObject, GameObject, float> OnHit;
    public static Action<GameObject, GameObject, float> OnTakeDamage;
    public static Action<GameObject, GameObject> OnKill;
    public static Action<GameObject> OnLose;
    public static Action<GameObject> OnWin;
    public static Action<GameObject> OnDraw;
    public static Action<GameObject> OnRestart;

    public static Action<string> StartLoadScene;

    public static void InvokeOnHit(GameObject hitter, GameObject target, float damage)
    {
        if (OnHit != null)
        {
            OnHit.Invoke(hitter, target, damage);
        }
    }

    public static void InvokeOnTakeDamage(GameObject hitter, GameObject target, float damage)
    {
        if (OnTakeDamage != null)
        {
            OnTakeDamage.Invoke(hitter, target, damage);
        }
    }

    public static void InvokeOnKill(GameObject killer, GameObject target)
    {
        if (OnKill != null)
        {
            OnKill.Invoke(killer, target);
        }
    }

    public static void InvokeOnLose(GameObject player)
    {
        if (OnLose != null)
        {
            OnLose.Invoke(player);
        }
    }

    public static void InvokeOnWin(GameObject player)
    {
        if (OnWin != null)
        {
            OnWin.Invoke(player);
        }
    }

    public static void InvokeOnDraw(GameObject player)
    {
        if (OnDraw != null)
        {
            OnDraw.Invoke(player);
        }
    }

    public static void InvokeOnRestart(GameObject player)
    {
        if (OnRestart != null)
        {
            OnRestart.Invoke(player);
        }
    }
    
    public static void InvokeStartLoadScene(string scene_name)
    {
        if (StartLoadScene != null)
        {
            StartLoadScene.Invoke(scene_name);
        }
    }
}
