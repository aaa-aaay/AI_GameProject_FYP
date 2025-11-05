using UnityEngine;

public class HitForceKnockback : MonoBehaviour
{
    [SerializeField] private float knockback_force;

    private Rigidbody rigid_body;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid_body = GetComponent<Rigidbody>();

        EventHolder.OnHit += WhenHit;
    }

    private void OnDestroy()
    {
        EventHolder.OnHit -= WhenHit;
    }

    private void WhenHit(GameObject hitter, GameObject target, float damage)
    {
        if (target == gameObject)
        {
            rigid_body.AddForce(((transform.position - hitter.transform.position).normalized + Vector3.up).normalized * knockback_force, ForceMode.Impulse);
        }
    }
}
