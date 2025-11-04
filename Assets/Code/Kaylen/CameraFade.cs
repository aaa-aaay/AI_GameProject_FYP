using UnityEngine;
using System.Collections.Generic;

public class CameraFade : MonoBehaviour
{
    [Header("Force Field Settings")]
    public float radius = 3f;
    [Range(0f, 1f)] public float fadeAlpha = 0.25f;
    public float fadeSpeed = 5f;

    // Internal caches
    private Dictionary<Renderer, Material[]> materialInstances = new Dictionary<Renderer, Material[]>();
    private Dictionary<Material, float> currentAlphas = new Dictionary<Material, float>();

    // Property name in shader
    private readonly string alphaProperty = "_Alpha";

    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        HashSet<Renderer> overlappedRenderers = new HashSet<Renderer>();

        // Handle overlapping renderers
        foreach (var hit in hits)
        {
            Renderer rend = hit.GetComponent<Renderer>();
            if (rend == null) continue;
            overlappedRenderers.Add(rend);

            // Initialize materials once
            if (!materialInstances.ContainsKey(rend))
            {
                Material[] mats = rend.materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    Material newMat = new Material(mats[i]);
                    mats[i] = newMat;
                    if (!currentAlphas.ContainsKey(newMat))
                        currentAlphas[newMat] = GetInitialAlpha(newMat);
                }
                rend.materials = mats;
                materialInstances[rend] = mats;
            }

            // Fade down to fadeAlpha
            foreach (Material mat in materialInstances[rend])
            {
                if (!mat.HasProperty(alphaProperty)) continue;

                float current = currentAlphas[mat];
                current = Mathf.Lerp(current, fadeAlpha, Time.deltaTime * fadeSpeed);
                currentAlphas[mat] = current;
                mat.SetFloat(alphaProperty, current);
            }
        }

        // Restore alpha for renderers no longer overlapping
        foreach (var kvp in materialInstances)
        {
            Renderer rend = kvp.Key;
            if (rend == null) continue;
            if (overlappedRenderers.Contains(rend)) continue;

            foreach (Material mat in kvp.Value)
            {
                if (!mat.HasProperty(alphaProperty)) continue;

                float current = currentAlphas[mat];
                current = Mathf.Lerp(current, 1f, Time.deltaTime * fadeSpeed);
                currentAlphas[mat] = current;
                mat.SetFloat(alphaProperty, current);
            }
        }
    }

    // Read initial alpha from the material
    private float GetInitialAlpha(Material mat)
    {
        return mat.HasProperty(alphaProperty) ? mat.GetFloat(alphaProperty) : 1f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
