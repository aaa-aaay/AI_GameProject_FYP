using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class ProximityFade : MonoBehaviour
{
    [Header("Detection Settings")]
    public float radius = 3f;

    [Header("Fade Settings")]
    public float fadeSpeed = 3f;
    public string alphaProperty = "_Alpha"; // shader alpha property

    [Header("Target Objects")]
    public List<Renderer> targetObjects = new List<Renderer>();

    // Internal caches
    private Dictionary<Renderer, Material[]> materialInstances = new Dictionary<Renderer, Material[]>();
    private Dictionary<Material, float> currentAlphas = new Dictionary<Material, float>();

    private void OnEnable()
    {
        foreach (Renderer rend in targetObjects)
        {
            if (rend == null || materialInstances.ContainsKey(rend)) continue;

            Material[] mats = rend.sharedMaterials;
            for (int i = 0; i < mats.Length; i++)
            {
                Material newMat = new Material(mats[i]);
                mats[i] = newMat;
                if (!currentAlphas.ContainsKey(newMat))
                    currentAlphas[newMat] = 1f; // start fully invisible
            }
            rend.materials = mats;
            materialInstances[rend] = mats;
        }
    }

    private void Update()
    {
        Vector3 playerPosXZ = new Vector3(transform.position.x, 0f, transform.position.z);

        foreach (Renderer rend in targetObjects)
        {
            if (rend == null || !materialInstances.TryGetValue(rend, out Material[] mats)) continue;

            BoxCollider box = rend.GetComponent<BoxCollider>();
            if (box == null) continue;

            // Closest point on BoxCollider (works for horizontal and vertical walls)
            Vector3 closest = box.ClosestPoint(transform.position);
            float distance = Vector3.Distance(playerPosXZ, new Vector3(closest.x, 0f, closest.z));

            bool isInRange = distance <= radius;
            float targetAlpha = isInRange ? 0f : 1f; 

            foreach (Material mat in mats)
            {
                if (mat == null || !mat.HasProperty(alphaProperty)) continue;

                float current = currentAlphas[mat];
                current = Mathf.Lerp(current, targetAlpha, Time.deltaTime * fadeSpeed);
                currentAlphas[mat] = current;
                mat.SetFloat(alphaProperty, current);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.7f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
