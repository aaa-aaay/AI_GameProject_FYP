using TMPro;
using Unity.InferenceEngine;
using Unity.VisualScripting;
using UnityEngine;

public class arrow : MonoBehaviour
{
    [SerializeField] private Transform tip;
    private Rigidbody rb;

    public bool launched { get; private set; } = false;
    private Vector3 windForce;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        SetCollision(true);
        launched = false;
    }

    public void Shoot(float strength, float yaw, float pitch, float windDirection, float windStrength)
    {
        if (launched) return;
        launched = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Quaternion angle = Quaternion.Euler(-pitch, yaw, 0f);
        Vector3 direction = angle * Vector3.forward;

        rb.AddForce(direction * strength, ForceMode.Impulse);

        windForce = Quaternion.Euler(0f, windDirection, 0f) * Vector3.forward * windStrength;
    }

    private void FixedUpdate()
    {
        if (!launched) return;

        if (rb.linearVelocity.sqrMagnitude > Mathf.Epsilon)
        {
            Quaternion targetRot = Quaternion.LookRotation(rb.linearVelocity.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 20f * Time.fixedDeltaTime);
        }

        rb.AddForce(windForce, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.TryGetComponent(out target target) && collision.gameObject.layer != LayerMask.NameToLayer("Floor"))
            return;

        SetCollision(false);

        ContactPoint contact = collision.GetContact(0);

        Vector3 desiredTipPos = contact.point - contact.normal * 0.1f;
        Vector3 delta = desiredTipPos - tip.position;
        transform.position += delta;

        transform.parent = collision.gameObject.transform;

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
