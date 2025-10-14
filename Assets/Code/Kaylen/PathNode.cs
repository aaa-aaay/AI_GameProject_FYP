using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    [Header("Graph Setup")]
    public List<PathNode> neighbors = new List<PathNode>();

    [Header("Gizmo Settings")]
    public Color idleColor = Color.gray;      
    public Color activeColor = Color.green;  
    public float nodeRadius = 0.3f;

    private void OnDrawGizmos()
    {
     
        Runner runner = FindFirstObjectByType<Runner>();

   
        Color drawColor = idleColor;
        if (runner != null && runner.path != null && runner.pathIndex < runner.path.Count)
        {
            if (runner.path[runner.pathIndex] == this)
            {
                drawColor = activeColor; // Runner is heading to this node
            }
        }

     
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
