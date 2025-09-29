using Unity.InferenceEngine;
using UnityEngine;

public class arrow : MonoBehaviour
{
    private Rigidbody rb;

    public bool launched { get; private set; } = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        SetCollision(true);
        launched = false;
    }

    public void Shoot(float strength, float yaw, float pitch)
    {
        if (launched) return;
        launched = true;

        Quaternion angle = Quaternion.Euler(-pitch, yaw, 0f);
        Vector3 direction = angle * Vector3.forward;

        rb.AddForce(direction * strength, ForceMode.Impulse);

        Debug.Log($"Force: {strength} \t Yaw: {yaw} Pitch: {pitch} \t Direction: {direction}");
    }

    private void FixedUpdate()
    {
        if (!launched) return;

        if (rb.linearVelocity.sqrMagnitude > Mathf.Epsilon)
        {
            Quaternion targetRot = Quaternion.LookRotation(rb.linearVelocity.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 20f * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.TryGetComponent(out target target) && other.transform.gameObject.layer != LayerMask.NameToLayer("Floor"))
            return;

        SetCollision(false);

        var collision = other.GetComponent<Collision>();

        ContactPoint contact = collision.GetContact(0);
        int point;

        if (target) point = target.OnHit(contact); else point = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent(out target target) && collision.gameObject.layer != LayerMask.NameToLayer("Floor"))
            return;

        SetCollision(false);

        ContactPoint contact = collision.GetContact(0);
        int point;

        if (target) point = target.OnHit(contact); else point = 0;

        archery_handler.instance.OnHit(point);
    }

    private void SetCollision(bool active)
    {
        if (active)
        {
            rb.isKinematic = false;
            GetComponentInChildren<CapsuleCollider>().enabled = true;
        }
        else
        {
            rb.isKinematic = true;
            GetComponentInChildren<CapsuleCollider>().enabled = false;
        }
    }
}
