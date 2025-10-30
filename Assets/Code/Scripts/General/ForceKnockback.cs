using UnityEngine;

public class ForceKnockback : MonoBehaviour
{
    [SerializeField] private float knockback_force;

    private Rigidbody rigid_body;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid_body = GetComponent<Rigidbody>();

        EventHolder.OnTakeDamage += WhenHit;
    }

    private void OnDestroy()
    {
        EventHolder.OnTakeDamage -= WhenHit;
    }

    private void WhenHit(GameObject hitter, GameObject target, float damage)
    {
        if (target == gameObject)
        {
            rigid_body.AddForce((transform.position - hitter.transform.position).normalized * knockback_force, ForceMode.Impulse);
        }
    }
}
