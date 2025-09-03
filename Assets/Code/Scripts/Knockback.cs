using UnityEngine;

public class Knockback : MonoBehaviour
{
    [SerializeField] private float force;

    private Rigidbody rigidbody;

    private void Start()
    {
        EventHandler.GotHit += inflict_knockback;
        rigidbody = GetComponent<Rigidbody>();
    }

    public void inflict_knockback(GameObject hitter, GameObject target, float damage)
    {
        if (target == gameObject)
        {
            Vector3 direction = target.transform.position - hitter.transform.position;
            direction.Normalize();
            rigidbody.AddForce(direction * force, ForceMode.Impulse);
        }
    }
}
