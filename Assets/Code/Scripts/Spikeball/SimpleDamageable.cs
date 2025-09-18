using UnityEngine;

public class SimpleDamageable : MonoBehaviour
{
    [SerializeField] protected float max_health = 5;
    [SerializeField] protected float damage_cooldown = 1;

    protected float health;
    protected float time_passed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = max_health;
        time_passed = damage_cooldown;

        EventHandler.GotHit += handle_hit;
        EventHandler.TookDamage += take_damage;
        EventHandler.EndScenario += reset_damageable;
        EventHandler.RestartGame += reset_damageable;
    }

    public float get_health()
    {
        return health;
    }

    protected void Update()
    {
        time_passed += Time.deltaTime;
    }

    protected virtual void OnDestroy()
    {
        EventHandler.GotHit -= handle_hit;
        EventHandler.TookDamage -= take_damage;
        EventHandler.GotKill -= reset_health;
        EventHandler.EndScenario -= reset_damageable;
        EventHandler.RestartGame -= reset_damageable;
    }

    public virtual void handle_hit(GameObject hitter, GameObject target, float damage)
    {
        if (target == gameObject)
        {
            if (time_passed >= damage_cooldown)
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
            //print(gameObject.name + " took damage");
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

    public virtual void reset_health(GameObject killer, GameObject target)
    {
        health = max_health;
    }

    public virtual void reset_damageable()
    {
        health = max_health;
    }

    public virtual void reset_damageable(GameObject target)
    {
        if (target == gameObject)
        {
            health = max_health;
        }
    }

    public bool is_alive()
    {
        return health > 0;
    }
}
