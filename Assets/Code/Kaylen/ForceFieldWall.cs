using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
public class ForceFieldWall : MonoBehaviour
{
    public static List<ForceFieldWall> AllWalls = new List<ForceFieldWall>();

    private MeshRenderer meshRenderer;
    private Collider meshCollider;
    private Material materialReference;

    [Header("Fade Settings")]
    [Range(0f, 1f)] public float minAlpha = 0f;
    [Range(0f, 1f)] public float maxAlpha = 0.6f;
    public float fadeSpeed = 10f;

    private string alphaPropertyName = "_Alpha";
    private float targetAlpha = 0f;
    private float currentAlpha = 0f;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<Collider>();
        materialReference = meshRenderer.sharedMaterial;

        AllWalls.Add(this);

        // Ensure the material starts invisible
        ApplyAlphaToMaterial(minAlpha);
        currentAlpha = minAlpha;
    }

    void OnDestroy()
    {
        AllWalls.Remove(this);
    }

    public Collider GetCollider() => meshCollider;

    public void SetOverlapT(float t)
    {
        // Smooth alpha target based on overlap
        targetAlpha = Mathf.Lerp(minAlpha, maxAlpha, t);
    }

    void Update()
    {
        // Smoothly interpolate alpha
        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
        ApplyAlphaToMaterial(currentAlpha);
    }

    private void ApplyAlphaToMaterial(float alpha)
    {
        if (materialReference == null) return;

        if (materialReference.HasProperty(alphaPropertyName))
            materialReference.SetFloat(alphaPropertyName, alpha);
    }
}
