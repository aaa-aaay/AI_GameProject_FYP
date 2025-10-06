using Unity.MLAgents.Integrations.Match3;
using UnityEngine;

public class BadmintionMovement : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] GameObject _meshToMove;
    [SerializeField] float _rotationSpeed;

    private Vector2 direction;

    public void Walk(bool start, Vector2 dir)
    {
        _animator.SetBool("walking", start);
        direction = dir;
    }


    private void FixedUpdate()
    {

        //if (direction != Vector2.zero) {

        //    // Convert 2D input (x,y) into 3D world direction (x,0,z)
        //    Vector3 moveDir = new Vector3(direction.x, 0f, direction.y).normalized;

        //    // Create a target rotation looking toward moveDir
        //    Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);

        //    // Smoothly rotate the mesh toward the target rotation
        //    _meshToMove.transform.rotation = Quaternion.Slerp(
        //        _meshToMove.transform.rotation,
        //        targetRotation,
        //        _rotationSpeed * Time.fixedDeltaTime
        //    );
        //}
    }

}
