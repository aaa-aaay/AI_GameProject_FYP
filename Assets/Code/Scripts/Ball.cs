using UnityEngine;

public class Ball : MonoBehaviour
{
    private GameObject player;
    private Rigidbody rigidbody;
    private bool moving;
    private Vector3 start_position;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        EventHandler.TurnStart += set_player;
        EventHandler.EndScenario += reset_ball;

        rigidbody = GetComponent<Rigidbody>();
        player = null;
        start_position = transform.localPosition;
    }

    private void OnDestroy()
    {
        EventHandler.TurnStart -= set_player;
        EventHandler.EndScenario -= reset_ball;
    }

    public void reset_ball()
    {
        rigidbody.linearVelocity = Vector3.zero;
        transform.localPosition = start_position;
    }

    public void shoot(Vector2 direction, float force)
    {
        rigidbody.linearVelocity = Vector3.zero;
        rigidbody.AddForce(new Vector3(direction.x, 0, direction.y) * force, ForceMode.Impulse);
        moving = true;
    }

    public void set_position(Vector2 new_position)
    {
        transform.position = new Vector3(new_position.x, 0.5f, new_position.y);
    }

    public void set_player(GameObject new_player, bool can_set_position)
    {
        player = new_player;
    }

    private void Update()
    {
        if (rigidbody.linearVelocity.magnitude <= 0.1f && moving)
        {
            moving = false;
            print("Done moving");
            EventHandler.InvokeTurnEnd(player);
        }
    }
}
