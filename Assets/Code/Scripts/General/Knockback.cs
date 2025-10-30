using UnityEngine;

public class Knockback : MonoBehaviour
{
    [SerializeField] private float knockback_force;
    [SerializeField] private float knockback_duration;

    private Movement movement;

    private Vector3 direction;

    private float time_passed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movement = GetComponent<Movement>();

        direction = Vector3.zero;

        time_passed = 0;

        EventHolder.OnTakeDamage += WhenHit;
    }

    private void OnDestroy()
    {
        EventHolder.OnTakeDamage -= WhenHit;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        time_passed += Time.deltaTime;

        if (time_passed > knockback_duration)
        {
            time_passed = knockback_duration;
        }

        movement.AddDirection(Vector3.Slerp(direction * knockback_force, Vector3.zero, time_passed / knockback_duration));
    }

    private void WhenHit(GameObject hitter, GameObject target, float damage)
    {
        if (target == gameObject)
        {
            direction = transform.position - hitter.transform.position;
            direction.y = 0;
            time_passed = 0;
        }
    }
}
