using Unity.MLAgents;
using UnityEngine;

public abstract class AgentDLC : Agent
{
    protected virtual void OnHit(GameObject hitter, GameObject target, float damage) { }
    protected virtual void OnTakeDamage(GameObject hitter, GameObject target, float damage) { }
    protected virtual void OnKill(GameObject killer, GameObject target) { }
    protected virtual void OnLose(GameObject player) { }
    protected virtual void OnWin(GameObject player) { }
    protected virtual void OnRestart(GameObject player) { }
}
