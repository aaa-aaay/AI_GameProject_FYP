using UnityEngine;

public class Damageable : SimpleDamageable
{
    [SerializeField] private Teams starting_team;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = max_health;
        time_passed = damage_cooldown;

        if (TeamSingleton.instance != null)
        {
            TeamSingleton.instance.join_team(gameObject, starting_team);
        }

        EventHandler.GotHit += handle_hit;
        EventHandler.TookDamage += take_damage;
        EventHandler.GotKill += reset_health;
        EventHandler.EndScenario += reset_damageable;
    }

    protected override void OnDestroy()
    {
        EventHandler.GotHit -= handle_hit;
        EventHandler.TookDamage -= take_damage;
        EventHandler.GotKill -= reset_health;
        EventHandler.EndScenario -= reset_damageable;
    }

    public override void handle_hit(GameObject hitter, GameObject target, float damage)
    {
        if (target == gameObject)
        {
            if (TeamSingleton.instance.check_same_team(hitter, target))
            {
                EventHandler.InvokePunish(hitter, -2);
            }
            else if (time_passed >= damage_cooldown)
            {
                EventHandler.InvokeTookDamage(hitter, target, damage);
            }
        }
    }

    public override void reset_health(GameObject killer, GameObject target)
    {
        if (killer == target)
        {
            if (killer == gameObject)
            {
                EventHandler.InvokePunish(gameObject, -1);
            }
        }
        else if (killer == gameObject)
        {
            heal_damage(2);
        }
        else if (target == gameObject)
        {
            if (TeamSingleton.instance.get_team(killer) != Teams.None)
            {
                health = max_health;
            }
        }
    }

    public override void reset_damageable()
    {
        TeamSingleton.instance.change_team(gameObject, starting_team, false);
        health = max_health;
    }

    public override void reset_damageable(GameObject target)
    {
        if (target == gameObject)
        {
            TeamSingleton.instance.change_team(gameObject, starting_team, false);
            health = max_health;
        }
    }
}
