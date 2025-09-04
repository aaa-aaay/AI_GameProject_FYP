using UnityEngine;

public class SmashShot : Shot
{

    [Header("Smash Tuning")]
    [SerializeField] private float travelTime = 0.35f;   // fast
    [SerializeField] private float arcHeight = 0.8f;    // small, quick “pop” near start
    [SerializeField] private float downBias = 2.5f;    // adds extra downward pull
    [SerializeField] private float skewPower = 0.35f;   // < 1 => peak earlier (steeper)

    [Header("Physics")]
    [SerializeField] private bool faceVelocity = true;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        if (!isFlying) return;

        float t = Mathf.Clamp01(elapsedTime / travelTime);

        // Skew t so the peak happens early (steep downward later)
        float ts = Mathf.Pow(t, skewPower);

        // Base horizontal lerp
        Vector3 pos = Vector3.Lerp(startPos, targetPos, t);

        // Small early "pop" upward (parabola peaked by skew)
        float bump = 4f * ts * (1f - ts) * arcHeight;

        // Extra downward pull as time progresses (slam feel)
        float slam = -downBias * t * t;

        // Final height
        pos.y = Mathf.Lerp(startPos.y, targetPos.y, t) + bump + slam;

        if (faceVelocity && t < 1f)
        {
            // Face movement direction for nicer visuals
            Vector3 delta = pos - transform.position;
            if (delta.sqrMagnitude > 0.0001f)
                transform.forward = delta.normalized;
        }

        transform.position = pos;

        elapsedTime += Time.deltaTime;

        if (t >= 1f)
        {
            isFlying = false;
            if (rb)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }
    }



    public override void Cancel()
    {
        isFlying = false;
        StopAllCoroutines();
        if (rb)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }



}
