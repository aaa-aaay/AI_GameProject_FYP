using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

public class PongBall : MonoBehaviour
{
    [SerializeField] private VisualEffect bounce_vfx;
    [SerializeField] private float bounce_force;
    [SerializeField, Range(0f, 1f)] private float start_bounce_multiplier;
    [SerializeField] private float max_speed;
    [SerializeField] private float bounce_tolerance;
    [SerializeField] private string add_force_tag;

    private Rigidbody rigid_body;

    private Vector3 start_position;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid_body = GetComponent<Rigidbody>();

        start_position = transform.position;

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
        bounce_vfx.transform.position = collision.GetContact(0).point;
        bounce_vfx.Play();

        if (collision.gameObject.CompareTag(add_force_tag))
        {
            if (Random.Range(0, 2) == 0)
            {
                ServiceLocator.Instance.GetService<AudioManager>().PlaySFX("Fireball", Camera.main.transform.position);
            }
            else
            {
                ServiceLocator.Instance.GetService<AudioManager>().PlaySFX("Fireball2", Camera.main.transform.position);
            }
            rigid_body.AddForce((transform.position - collision.transform.position).normalized * bounce_force, ForceMode.Impulse);
        }
        else
        {
            rigid_body.AddForce(rigid_body.linearVelocity.normalized * bounce_force, ForceMode.Impulse);
        }
        if (rigid_body.linearVelocity.sqrMagnitude > max_speed * max_speed)
        {
            rigid_body.linearVelocity = rigid_body.linearVelocity.normalized * max_speed;
        }

        Vector3 direction = rigid_body.linearVelocity.normalized;
        float speed = rigid_body.linearVelocity.magnitude;

        if (direction.z > -bounce_tolerance && direction.z < bounce_tolerance)
        {
            if (direction.z < 0)
            {
                direction.z -= bounce_tolerance;
            }
            else
            {
                direction.z += bounce_tolerance;
            }
            rigid_body.linearVelocity = direction.normalized * speed;
        }
    }

    public void Restart()
    {
        ServiceLocator.Instance.GetService<AudioManager>().PlaySFX("BMT_Score", Camera.main.transform.position);
        rigid_body.linearVelocity = Vector3.zero;
        rigid_body.AddForce(new Vector3(Random.Range(-1, 1f), 0, Random.Range(-1, 1f)).normalized * max_speed * start_bounce_multiplier, ForceMode.Impulse); 
        transform.position = start_position;
    }

    public void Restart(GameObject player)
    {
        if (player == gameObject)
        {
            Restart();
        }
    }
}
