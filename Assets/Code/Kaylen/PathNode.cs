using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    [Header("Graph Setup")]
    public List<PathNode> neighbors = new List<PathNode>();

    [Header("Gizmo Settings")]
    public Color idleColor = Color.gray;      // Default color
    public Color activeColor = Color.green;   // Runner is heading here
    public float nodeRadius = 0.3f;

    private void OnDrawGizmos()
    {
        // Find the runner in the scene
        Runner runner = FindFirstObjectByType<Runner>();

        // Decide color
        Color drawColor = idleColor;
        if (runner != null && runner.path != null && runner.pathIndex < runner.path.Count)
        {
            if (runner.path[runner.pathIndex] == this)
            {
                drawColor = activeColor; // Runner is heading to this node
            }
        }

        // Draw this node
        Gizmos.color = drawColor;
        Gizmos.DrawSphere(transform.position, nodeRadius);

        // Draw connections to neighbors
        Gizmos.color = Color.white;
        foreach (PathNode neighbor in neighbors)
        {
            if (neighbor != null)
            {
                Gizmos.DrawLine(transform.position, neighbor.transform.position);
            }
        }
    }
}
