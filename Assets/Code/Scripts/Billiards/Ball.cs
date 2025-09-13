using UnityEngine;
using System.Collections.Generic;

public class Ball : MonoBehaviour
{
    [SerializeField] protected bool scored;
    [SerializeField] protected LayerMask layers;
    [SerializeField] protected List<GameObject> scoring_area;
    [SerializeField] protected List<GameObject> penalty_area;

    protected Vector3 start_position;
    protected Vector3 last_position;
    protected Rigidbody rigidbody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        start_position = transform.position;
        last_position = transform.position;
        rigidbody = GetComponent<Rigidbody>();

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
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
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
            scored = false;
            transform.position = start_position;
            last_position = transform.position;
            rigidbody.isKinematic = false;
        }
    }
}
