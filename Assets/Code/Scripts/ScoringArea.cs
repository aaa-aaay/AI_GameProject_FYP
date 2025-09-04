using System.Collections.Generic;
using UnityEngine;

public class ScoringArea : MonoBehaviour
{
    [SerializeField] protected GameObject cue_ball;
    [SerializeField] protected List<GameObject> balls;

    private List<GameObject> scored;

    protected GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player = null;
        scored = new List<GameObject>();

        EventHandler.TurnStart += set_player;
    }

    private void OnDestroy()
    {
        EventHandler.TurnStart -= set_player;
    }

    public void set_player(GameObject new_player, bool can_set_position)
    {
        player = new_player;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (balls.Contains(collision.gameObject))
        {
            EventHandler.InvokeScored(player);

            scored.Add(collision.gameObject);

            print(scored.Count + " vs " + balls.Count);
            if (scored.Count >= balls.Count)
            {
                scored.Clear();
                EventHandler.InvokeEndScenario();
            }
        }
    }

    protected void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject == cue_ball)
        {
            EventHandler.InvokeOutOfBounds(player);
        }
    }
}
