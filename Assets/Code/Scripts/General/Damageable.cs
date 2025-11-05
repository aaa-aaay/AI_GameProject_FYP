using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] protected float max_health;
    [SerializeField] protected float damage_cooldown;

    protected float time_passed;
    protected float current_health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DamageableStart();

        EventHolder.OnHit += WhenHit;
        EventHolder.OnTakeDamage += TakeDamage;
        EventHolder.OnRestart += RestartGame;
    }

    protected void DamageableStart()
    {
        time_passed = 0;
        current_health = max_health;
    }

    // Update is called once per frame
    void Update()
    {
        time_passed += Time.deltaTime;
    }

    public virtual void WhenHit(GameObject hitter, GameObject target, float damage)
    {
        if (target == gameObject)
        {
            if (time_passed > damage_cooldown)
            {
                EventHolder.InvokeOnTakeDamage(hitter, target, damage);
            }
        }
    }

    public virtual void TakeDamage(GameObject hitter, GameObject target, float damage)
    {
        if (target == gameObject)
        {
            time_passed = 0;

            current_health -= damage;

            print(gameObject.name + " " + current_health);

            if (current_health <= 0)
            {
                EventHolder.InvokeOnKill(hitter, target);
            }
        }
    }

    public virtual void RestartGame(GameObject player)
    {
        if (player == gameObject)
        {
            DamageableStart();
        }
    }

    public bool IsAlive()
    {
        return current_health > 0;
    }

    public bool CanDamage()
    {
        return time_passed >= damage_cooldown;
    }

    public float GetHealth()
    {
        return current_health;
    }

}
