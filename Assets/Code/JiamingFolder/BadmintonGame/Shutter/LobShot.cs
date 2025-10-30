using UnityEngine;

public class LobShot : Shot
{
    [SerializeField] private float arcHeight = 3.0f;

    private void Update()
    {
        if (!isFlying) return;

        elapsedTime += Time.deltaTime;
        float t = elapsedTime / travelTime;

        // Parabolic curve
        Vector3 pos = Vector3.Lerp(startPos, targetPos, t);
        pos.y += Mathf.Sin(Mathf.PI * t) * arcHeight;

        transform.position = pos;
        UpdateRotation(pos);

        if (t >= 1f)
            isFlying = false;
    }
}
