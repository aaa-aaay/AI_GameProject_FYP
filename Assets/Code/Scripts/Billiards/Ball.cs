using UnityEngine;
using System.Collections.Generic;

public class Ball : MonoBehaviour
{
    [SerializeField] protected bool scored;
    [SerializeField] protected LayerMask layers;
    [SerializeField] protected List<GameObject> scoring_area;
    [SerializeField] protected List<GameObject> penalty_area;
    [SerializeField] protected Vector3 vel;

    protected Vector3 start_position;
    protected Vector3 last_position;
    protected Rigidbody rigidbody;
    protected float last_distance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        start_position = transform.position;
        last_position = transform.position;
        rigidbody = GetComponent<Rigidbody>();

        last_distance = Mathf.Infinity;

        EventHandler.Scored += when_scored;
        EventHandler.OutOfBounds += when_out_of_bounds;
        EventHandler.RestartGame += restart_game;
    }

    private void OnDestroy()
    {
        EventHandler.Scored -= when_scored;
        EventHandler.OutOfBounds -= when_out_of_bounds;
        EventHandler.RestartGame -= restart_game;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!scored)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, transform.localScale.y + 0.1f, layers))
            {
                last_position = hit.point;
                last_position.y += transform.localScale.y * 0.5f;
            }

            if (rigidbody.linearVelocity.magnitude <= 0.5f && rigidbody.linearVelocity != Vector3.zero)
            {
                List<GameObject> holes = BilliardSingleton.instance.get_holes(gameObject);

                float shortest_distance = Mathf.Infinity;
                float distance;

                for (int i = 0; i < holes.Count; i++)
                {
                    distance = Vector3.Distance(transform.position, holes[i].transform.position);
                    if (distance < shortest_distance)
                    {
                        shortest_distance = distance;
                    }
                }

                if (shortest_distance < last_distance)
                {
                    BilliardSingleton.instance.reward_player(gameObject, 0.01f);
                }
                else
                {
                    BilliardSingleton.instance.reward_player(gameObject, -0.01f);
                }

                last_distance = shortest_distance;
            }
        }

        vel = rigidbody.linearVelocity;
    }

    protected virtual void OnCollisionStay(Collision collision)
    {
        if (!scored)
        {
            if (scoring_area.Contains(collision.gameObject))
            {
                EventHandler.InvokeScored(gameObject);
            }
            else if (penalty_area.Contains(collision.gameObject))
            {
                EventHandler.InvokeOutOfBounds(gameObject);
            }
        }
    }

    public virtual void when_scored(GameObject game_object)
    {
        if (gameObject == game_object)
        {
            scored = true;
            Vector3 temp = transform.position;
            temp.y = -10;
            transform.position = temp;
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.isKinematic = true;
        }
    }

    public virtual void when_out_of_bounds(GameObject game_object)
    {
        if (gameObject == game_object)
        {
            transform.position = last_position;
            rigidbody.linearVelocity = Vector3.zero;
        }
    }

    public void restart_game(GameObject game_object)
    {
        if (gameObject == game_object)
        {
            rigidbody.isKinematic = false;
            scored = false;
            transform.position = start_position;
            last_position = transform.position;
        }
    }
}
