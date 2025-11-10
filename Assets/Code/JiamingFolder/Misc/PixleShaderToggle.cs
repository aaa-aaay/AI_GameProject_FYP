using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PixleShaderToggle : MonoBehaviour
{
    [SerializeField] private UniversalRendererData rendererData; 
    private ScriptableRendererFeature pixelationFeature;
    private Material pixelationMat;

    private void Start()
    {
        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature.name == "FullScreenPassRendererFeature") // name in the Renderer Feature list
            {
                pixelationFeature = feature;
                // Access material if public (depends on implementation)
                var type = feature.GetType();
                var field = type.GetField("m_Material", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                pixelationMat = field?.GetValue(feature) as Material;
                break;
            }
        }

        //SetPixelation(false);
    }

    public void SetPixelation(bool enable)
    {
        if (pixelationFeature != null)
            pixelationFeature.SetActive(enable); // Toggles the effect ON/OFF fully
    }
}
