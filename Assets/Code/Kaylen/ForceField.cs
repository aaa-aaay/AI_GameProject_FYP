using UnityEngine;

public class ForceFieldController : MonoBehaviour
{
    [Header("Force Field Settings")]
    public float radius = 3f;
    public bool showDebug = true;
    public float checkInterval = 0.05f;

    private float checkTimer = 0f;

    void Update()
    {
        checkTimer -= Time.deltaTime;
        if (checkTimer <= 0f)
        {
            UpdateWalls();
            checkTimer = checkInterval;
        }
    }

    void UpdateWalls()
    {
        Vector3 center = transform.position;

        // Get all colliders overlapping the sphere
        Collider[] hits = Physics.OverlapSphere(center, radius);

        foreach (var wall in ForceFieldWall.AllWalls)
        {
            if (wall == null) continue;
            Collider col = wall.GetCollider();
            if (col == null) continue;

            // Check if the wall collider is inside the overlap
            bool isOverlapping = System.Array.Exists(hits, hit => hit == col);

            float t = isOverlapping ? 1f : 0f; // full alpha when overlapping, 0 otherwise

            wall.SetOverlapT(t);

            if (showDebug)
            {
                // Draw debug sphere for overlapping walls
                if (isOverlapping)
                    Debug.DrawLine(center, col.bounds.ClosestPoint(center), Color.cyan);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
