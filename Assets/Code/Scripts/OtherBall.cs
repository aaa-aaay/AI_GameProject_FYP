using System.Collections.Generic;
using UnityEngine;

public class OtherBall : MonoBehaviour
{
    [SerializeField] private List<GameObject> bounds;

    private Rigidbody rigidbody;
    private Vector3 last_position;
    private Vector3 start_position;
    private bool out_of_bounds;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        EventHandler.OutOfBounds += check_out_of_bounds;
        EventHandler.EndScenario += reset_ball;
        out_of_bounds = false;
        start_position = transform.localPosition;
    }

    private void OnDestroy()
    {
        EventHandler.OutOfBounds -= check_out_of_bounds;
        EventHandler.EndScenario -= reset_ball;
    }

    public void reset_ball()
    {
        rigidbody.linearVelocity = Vector3.zero;
        transform.localPosition = start_position;
    }

    public void check_out_of_bounds(GameObject ball)
    {
        if (ball == gameObject)
        {
            out_of_bounds = true;
        }
    }

    public void on_turn_end(GameObject player)
    {
        if (out_of_bounds)
        {
            transform.position = last_position;
            rigidbody.linearVelocity = Vector3.zero;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (bounds.Contains(collision.gameObject))
        {
            if (bounds[bounds.IndexOf(collision.gameObject)].transform.position.y < transform.position.y)
            {
                last_position = transform.position;
            }
        }
    }
}
