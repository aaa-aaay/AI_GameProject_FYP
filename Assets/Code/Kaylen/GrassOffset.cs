using UnityEngine;

public class GrassOffset : MonoBehaviour
{
    [Header("Root parent containing all grass patches")]
    public Transform grassRoot;

    [Header("Offset Range")]
    public float offsetRange = 0.1f;

    [ContextMenu("Apply Random Offset")]
    public void ApplyRandomOffset()
    {
        if (grassRoot == null)
        {
            Debug.LogWarning("[GrassOffset] No grass root assigned!");
            return;
        }

        int totalCount = 0;
        int lineCount = 0;

        // Loop through all "GrassLine" objects (4th level)
        foreach (Transform field in grassRoot) // Grass Fields
        {
            foreach (Transform patch in field) // Grass Patches
            {
                foreach (Transform line in patch) // Grass Lines
                {
                    // Generate one shared random X offset for this entire line
                    float xOffset = Random.Range(-offsetRange, offsetRange);
                    lineCount++;

                    // Apply that same X offset to all children (the individual grass)
                    foreach (Transform grass in line)
                    {
                        Vector3 pos = grass.localPosition;
                        pos.x += xOffset;
                        grass.localPosition = pos;
                        totalCount++;
                    }
                }
            }
        }

        Debug.Log($"[GrassOffset] Applied X-offsets to {totalCount} grass objects across {lineCount} lines under {grassRoot.name}.");
    }
}
