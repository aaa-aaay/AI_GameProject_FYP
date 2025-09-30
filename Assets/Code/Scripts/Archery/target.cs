using UnityEngine;

public class target : MonoBehaviour
{
    [SerializeField] private ScoreRing[] rings;

    public int OnHit(ContactPoint contact)
    {
        Vector3 hit = contact.point;
        float dist = Vector3.Distance(hit, transform.position);

        foreach (var ring in rings)
        {
            if (dist <= ring.radius)
            {
                return (ring.points);
            }
        }

        return (0);
    }
}

[System.Serializable]
public struct ScoreRing
{
    public float radius;
    public int points;
}