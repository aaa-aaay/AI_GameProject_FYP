using UnityEngine;

public class KnockbackTrigger : MonoBehaviour
{
    [SerializeField] private float force = 1;
    [SerializeField] private float time = 0.1f;
    [SerializeField] private float cooldown = 1;

    private float time_passed;
    private float cooldown_passed;
    private Vector3 direction;
    private Rigidbody rigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        time_passed = 0;
        cooldown_passed = 0;
        direction = Vector3.zero;
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        time_passed += Time.deltaTime;
        cooldown_passed += Time.deltaTime;
        if (time_passed < time)
        {
            rigidbody.linearVelocity += Vector3.Lerp(direction, Vector3.zero, time_passed / time) * force;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cooldown_passed >= cooldown)
        {
            time_passed = 0;
            cooldown_passed = 0;
            direction = transform.position - other.transform.position;
            direction.y = 0;
            direction.Normalize();
        }
    }
}
