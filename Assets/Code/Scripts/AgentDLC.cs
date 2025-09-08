using Unity.MLAgents;
using UnityEngine;

public class AgentDLC : Agent
{
    public void base_start()
    {
        EventHandler.Punish += punish;
    }

    public void punish(GameObject target, float punishment)
    {
        if (target == gameObject)
        {
            AddReward(punishment);
        }
    }
}
