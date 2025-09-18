using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class BilliardSingleton : MonoBehaviour
{
    public static BilliardSingleton instance;

    [SerializeField] private float max_force;
    [SerializeField] private List<BilliardData> data;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < data.Count; i++)
        {
            data[i].start();
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < data.Count; i++)
        {
            data[i].on_destroy();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < data.Count; i++)
        {
            data[i].fixed_update();
        }
    }

    public void shoot_ball(GameObject player, float x, float z, float force_percentage)
    {
        Vector2 temp = new Vector2(x, z);
        temp.Normalize();
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].is_player(player))
            {
                data[i].shoot_ball(temp.x * max_force * force_percentage, temp.y * max_force * force_percentage);
            }
        }
    }

    public void shoot_ball(GameObject player, Vector3 dir, float force_percentage)
    {
        shoot_ball(player, dir.x, dir.z, force_percentage);
    }

    public void set_position(GameObject player, float x, float z)
    {
        set_position(player, new Vector3(x, 0, z));
    }

    public void set_position(GameObject player, Vector3 new_position)
    {
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].is_player(player))
            {
                data[i].get_cue_ball().transform.localPosition = new_position;
            }
        }
    }

    public void reward_player(GameObject game_object, float reward)
    {
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].in_game(game_object)) 
            {
                data[i].reward_player(reward);
            }
        }
    }

    public Rigidbody get_cue_ball(GameObject game_object)
    {
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].in_game(game_object))
            {
                return data[i].get_cue_ball();
            }
        }
        return null;
    }

    public List<Rigidbody> get_balls(GameObject game_object)
    {
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].in_game(game_object))
            {
                return data[i].get_balls();
            }
        }
        return null;
    }

    public int get_score(GameObject game_object)
    {
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].in_game(game_object))
            {
                return data[i].get_score();
            }
        }
        return 0;
    }

    public List<GameObject> get_holes(GameObject game_object)
    {
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i].in_game(game_object))
            {
                return data[i].get_holes();
            }
        }
        return null;
    }
}
