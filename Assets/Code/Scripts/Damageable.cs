using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] private float max_health = 5;
    [SerializeField] private float damage_cooldown = 1;

    private float health;
    private float time_passed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = max_health;
        time_passed = damage_cooldown;

        EventHandler.GotHit += handle_hit;
        EventHandler.TookDamage += take_damage;
        EventHandler.GotKill += reset_health;
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
    }

    public void handle_hit(GameObject hitter, GameObject target, float damage)
    {
        if (target == gameObject)
        {
            if (time_passed >= damage_cooldown)
            {
                EventHandler.InvokeTookDamage(hitter, target, damage);
            }
        }
    }

    public void take_damage(GameObject hitter, GameObject target, float damage)
    {
        if (target == gameObject)
        {
            health -= damage;
            time_passed = 0;

            if (health <= 0)
            {
                EventHandler.InvokeGotKill(hitter, target);
            }
        }
    }

    public void reset_health(GameObject killer, GameObject target)
    {
        if (target == gameObject)
        {
            health = max_health;
        }
    }
}
