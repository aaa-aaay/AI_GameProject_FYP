using Unity.MLAgents;
using UnityEngine;

public abstract class AgentDLC : Agent
{
    private void Start()
    {
        base_start();
    }

    public void base_start()
    {
        EventHandler.Punish += punish;
    }

    public void base_destroy()
    {
        EventHandler.Punish -= punish;
    }

    public void punish(GameObject target, float punishment)
    {
        if (target == gameObject)
        {
            AddReward(punishment);
        }
    }

    // Returns a value from 0 - 1
    public float get_positive(float value)
    {
        value = Mathf.Clamp(value, -1, 1);
        value++;
        return value / 2;
    }
}
