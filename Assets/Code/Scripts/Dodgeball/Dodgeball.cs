using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class Dodgeball : MonoBehaviour
{
    [SerializeField] private float lifespan;
    [SerializeField] private float force;
    [SerializeField] private float offset;
    [SerializeField] private float half_width;
    [SerializeField] private float half_height;

    [SerializeField] private LayerMask hittable_layers;

    private List<GameObject> hits;

    private Rigidbody rigid_body;
    private SphereCollider sphere_collider;

    private float time_passed;
    private bool shot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid_body = GetComponent<Rigidbody>();
        sphere_collider = GetComponent<SphereCollider>();

        hits = new List<GameObject>();

        RestartGame();

        EventHolder.OnRestart += RestartGame;
    }

    // Update is called once per frame
    void Update()
    {
        if (shot)
        {
            time_passed += Time.deltaTime;

            if (time_passed > lifespan)
            {
                RestartGame();
            }

            Collider[] hit_objects = Physics.OverlapSphere(transform.position, sphere_collider.radius, hittable_layers);

            for (int i = 0; i < hit_objects.Length; i++)
            {
                if (!hits.Contains(hit_objects[i].gameObject))
                {
                    hits.Add(hit_objects[i].gameObject);
                    EventHolder.InvokeOnHit(gameObject, hit_objects[i].gameObject, 1);
                }
            }
        }
    }

    public void Shoot(Vector3 start_position, Vector3 direction)
    {
        if (!shot)
        {
            time_passed = 0;
            shot = true;
            hits.Clear();
            transform.position = start_position + direction.normalized * sphere_collider.radius * offset * 2;

            rigid_body.linearVelocity = Vector3.zero;
            rigid_body.AddForce(direction * force, ForceMode.Impulse);
        }
    }

    public bool GetShot()
    {
        return shot;
    }

    public void ShootForward(Transform start_transform)
    {
        Shoot(start_transform.position, start_transform.forward);
    }

    public Vector3 get_velocity()
    {
        return rigid_body.linearVelocity;
    }

    public float get_time_left()
    {
        if (time_passed > lifespan)
        {
            return 0;
        }
        return lifespan - time_passed;
    }

    protected void RestartGame()
    {
        time_passed = 0;
        shot = false;
        transform.localPosition = new Vector3(Random.Range(-half_width, half_width), 0.5f, Random.Range(-half_height, half_height));
    }

    protected void RestartGame(GameObject player)
    {
        if (player == gameObject)
        {
            RestartGame();
        }
    }
}
