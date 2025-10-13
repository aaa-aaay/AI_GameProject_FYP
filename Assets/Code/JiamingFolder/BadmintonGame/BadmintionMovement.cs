using Unity.MLAgents.Integrations.Match3;
using UnityEngine;

public class BadmintionMovement : MonoBehaviour
{
    [SerializeField] Animator _animator;
    private Rigidbody _rb;

    [Header("Movement Settings")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.2f;


    private bool _isDashing;
    private float _dashTimer;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
        _animator = null;
        _rb = null;

    }
    public void Walk(bool start)
    {
        _animator.SetBool("walking", start);
    }

    public void Dash(Vector3 dir)
    {
        if (_isDashing) return; // prevent overlapping dashes

        _isDashing = true;
        _dashTimer = dashDuration;

        // reset velocity so dash is consistent
        _rb.linearVelocity = Vector3.zero;

        // apply instant impulse
        _rb.AddForce(dir.normalized * dashForce, ForceMode.VelocityChange);

    }


    private void FixedUpdate()
    {

        if (_isDashing)
        {
            _dashTimer -= Time.fixedDeltaTime;
            if (_dashTimer <= 0f)
            {
                _isDashing = false;
                // stop dash movement smoothly
                _rb.linearVelocity = Vector3.zero;
            }
        }

    }

}
