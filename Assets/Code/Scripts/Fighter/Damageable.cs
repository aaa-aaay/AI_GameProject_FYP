using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] private float max_health = 5;
    [SerializeField] private float damage_cooldown = 1;
    [SerializeField] private Teams starting_team;

    private float health;
    private float time_passed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = max_health;
        time_passed = damage_cooldown;

        TeamSingleton.instance.join_team(gameObject, starting_team);

        EventHandler.GotHit += handle_hit;
        EventHandler.TookDamage += take_damage;
        EventHandler.GotKill += reset_health;
        EventHandler.EndScenario += reset_damageable;
    }

    public float get_health()
    {
        return health;
    }

    private void Update()
    {
        time_passed += Time.deltaTime;
    }

    private void OnDestroy()
    {
        EventHandler.GotHit -= handle_hit;
        EventHandler.TookDamage -= take_damage;
        EventHandler.GotKill -= reset_health;
        EventHandler.EndScenario -= reset_damageable;
    }

    public void handle_hit(GameObject hitter, GameObject target, float damage)
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

    public void take_damage(GameObject hitter, float damage)
    {
        health -= damage;
        time_passed = 0;

        if (health <= 0)
        {
            EventHandler.InvokeGotKill(hitter, gameObject);
        }
    }

    public void take_damage(GameObject hitter, GameObject target, float damage)
    {
        if (target == gameObject)
        {
            print(gameObject.name + " took damage");
            take_damage(hitter, damage);
        }
    }

    public void heal_damage(float healing)
    {
        health += healing;
        if (health > max_health)
        {
            health = max_health;
        }
    }

    public void reset_health(GameObject killer, GameObject target)
    {
        if (killer == gameObject)
        {
            heal_damage(max_health);
        }
        if (target == gameObject)
        {
            health = max_health;
        }
    }

    public void reset_damageable()
    {
        TeamSingleton.instance.change_team(gameObject, starting_team, false);
        health = max_health;
    }
}
