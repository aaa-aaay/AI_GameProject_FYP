using Unity.MLAgents.Integrations.Match3;
using UnityEngine;

public class BadmintionMovement : MonoBehaviour
{
    [SerializeField] Animator _animator;

    public void Walk(bool start)
    {
        _animator.SetBool("walking", start);
    }


    private void FixedUpdate()
    {
    }

}
