using UnityEngine;
using UnityEngine.VFX;

public class ShutterFX : MonoBehaviour
{
    [SerializeField] private VisualEffect vfx;



    public void PlayHitEffects()
    {
        if (vfx != null)
        {
            vfx.Play();
            Debug.Log("Vfx Played");
        }
            



    }

    public void StopHitEffects()
    {
        if (vfx != null)
            vfx.Stop();
    }
}
