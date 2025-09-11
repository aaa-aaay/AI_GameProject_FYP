using Unity.VisualScripting;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] protected LayerMask raycast_layers;
    [SerializeField] protected GameObject scoring_area;
    [SerializeField] protected GameObject penalty_area;

    protected Vector3 start_position;
    protected Vector3 last_table_position;
    protected bool is_scored;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BilliardSingleton.instance.set_new_ball(gameObject, GetComponent<Rigidbody>());
        start_position = transform.localPosition;
        last_table_position = Vector3.zero;
        is_scored = false;

        EventHandler.OutOfBounds += out_of_bounds;
        EventHandler.EndScenario += restart;
    }

    private void OnDestroy()
    {
        EventHandler.OutOfBounds -= out_of_bounds;
        EventHandler.EndScenario -= restart;
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f, raycast_layers))
        {
            last_table_position = hit.point;
            last_table_position.y = 0;
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (BilliardSingleton.instance.is_ball(collision.gameObject))
        {
            BilliardSingleton.instance.reward_player(0.01f);
        }
        else
        {
            check_scored(collision.gameObject);
        }
    }

    public virtual void check_scored(GameObject collided)
    {
        if (!is_scored)
        {
            if (collided == scoring_area)
            {
                is_scored = true;
                EventHandler.InvokeScored(gameObject);
            }
            else if (collided == penalty_area)
            {
                EventHandler.InvokeOutOfBounds(gameObject);
            }
        }
    }

    public virtual void out_of_bounds(GameObject game_object)
    {
        if (gameObject == game_object)
        {
            transform.localPosition = last_table_position;
			BilliardSingleton.instance.get_ball_data(gameObject).get_rigidbody().linearVelocity = Vector3.zero;
        }
    }

    public virtual void restart()
    {
        transform.localPosition = start_position;
        BilliardSingleton.instance.get_ball_data(gameObject).get_rigidbody().linearVelocity = Vector3.zero;
        is_scored = false;
    }
}
