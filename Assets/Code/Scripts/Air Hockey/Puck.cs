using UnityEngine;

public class Puck : MonoBehaviour
{
    [SerializeField] private float bounce_force;
    [SerializeField, Range(0f, 1f)] private float start_bounce_multiplier;
    [SerializeField] private float max_speed;

    private Rigidbody rigid_body;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid_body = GetComponent<Rigidbody>();

        Restart();

        EventHolder.OnRestart += Restart;
    }

    private void FixedUpdate()
    {
        if (rigid_body.linearVelocity.magnitude < max_speed * 0.25f)
        {
            rigid_body.AddForce(rigid_body.linearVelocity.normalized * max_speed * 0.25f, ForceMode.Impulse);
        }
    }

    private void OnDestroy()
    {
        EventHolder.OnRestart -= Restart;
    }

    private void OnCollisionEnter(Collision collision)
    {
        PreviousPosition collided = collision.gameObject.GetComponent<PreviousPosition>();
        if (collided != null)
        {
            rigid_body.AddForce((collided.GetDirection().normalized + (transform.position - collision.transform.position).normalized).normalized * bounce_force, ForceMode.Impulse);
        }
        else
        {
            rigid_body.AddForce(rigid_body.linearVelocity.normalized * bounce_force, ForceMode.Impulse);
        }
        if (rigid_body.linearVelocity.sqrMagnitude > max_speed * max_speed)
        {
            rigid_body.linearVelocity = rigid_body.linearVelocity.normalized * max_speed;
        }
    }

    public void Restart()
    {
        rigid_body.linearVelocity = Vector3.zero;
        rigid_body.AddForce(new Vector3(Random.Range(-1, 1f), 0, Random.Range(-1, 1f)).normalized * max_speed * start_bounce_multiplier, ForceMode.Impulse);
    }

    public void Restart(GameObject player)
    {
        if (player == gameObject)
        {
            Restart();
        }
    }
}
