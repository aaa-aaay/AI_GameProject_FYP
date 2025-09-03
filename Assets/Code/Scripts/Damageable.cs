using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] private float max_health = 5;
    private float health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = max_health;
        EventHandler.GotHit += take_damage;
    }

    private void OnDestroy()
    {
        EventHandler.GotHit -= take_damage;
    }

    public void take_damage(GameObject hitter, GameObject target, float damage)
    {
        if (target == gameObject)
        {
            health -= damage;
            print(gameObject.name + " took damage");

            if (health <= 0)
            {
                EventHandler.InvokeGotKill(hitter, target);
            }
        }
    }
}
