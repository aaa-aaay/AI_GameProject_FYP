using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class trajectory_preview : MonoBehaviour
{
    [SerializeField] private Transform launchOrigin;

    [SerializeField, Min(4)] private int numSamples = 60;
    [SerializeField, Min(0f)] private float timePerSample = 0.05f;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private float tipPenetration = 0.05f;

    [SerializeField] private float forceToSpeed = 1f;

    private LineRenderer lr;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;
        lr.useWorldSpace = true;
    }

    public Vector3 ShowPath(Vector3 position, float force, float yaw, float pitch, float windDir = 0, float windSpeed = 0)
    {
        Vector3 p0 = position;
        Vector3 dir = Quaternion.Euler(-pitch, yaw, 0f) * Vector3.forward;
        Vector3 v = dir.normalized * (forceToSpeed * force);

        Vector3 a = Physics.gravity;
        if (windSpeed > float.Epsilon)
        {
            Vector3 windDirection = Quaternion.Euler(0f, windDir, 0f) * Vector3.forward;
            Vector3 windAcceleration = windDirection * windSpeed;
            a += windAcceleration;
        }

        var points = new List<Vector3>(numSamples);
        Vector3 p = p0;
        points.Add(p0);

        for (int i = 1; i < numSamples; i++)
        {
            // Integrate velocity & position
            v += a * timePerSample;
            Vector3 pNext = p + v * timePerSample;

            // Raycast segment to stop on impact
            if (Physics.Raycast(p, (pNext - p).normalized, out var hit, (pNext - p).magnitude, hitMask, QueryTriggerInteraction.Ignore))
            {
                // small penetration for nicer look
                Vector3 end = hit.point + (-hit.normal) * tipPenetration;
                points.Add(end);
                break;
            }

            points.Add(pNext);
            p = pNext;
        }

        lr.positionCount = points.Count;
        lr.SetPositions(points.ToArray());
        lr.enabled = true;
        return points.ToArray()[points.Count - 1];
    }

    public void Hide()
    {
        lr.enabled = false;
        lr.positionCount = 0;
    }
}
