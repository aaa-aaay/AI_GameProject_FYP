using UnityEngine;

public class DropShot : Shot
{
    [SerializeField] private float arcHeight = 3.0f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!isFlying) return;

        // compute t first, then increment time
        float t = Mathf.Clamp01(elapsedTime / travelTime);

        float bump = 4f * t * (1f - t) * arcHeight; // peak at mid
        Vector3 pos = Vector3.Lerp(startPos, targetPos, t);
        pos.y = Mathf.Lerp(startPos.y, targetPos.y, t) + bump;

        transform.position = pos;
        UpdateRotation(pos);

        elapsedTime += Time.deltaTime;

        if (t >= 1f)
        {
            isFlying = false;

            // re-enable physics if needed after landing
            if (rb)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }
    }
}
