using UnityEngine;
using UnityEngine.VFX;

public class ShutterFX : MonoBehaviour
{
    [SerializeField] private GameObject _vfxOBJ;
    private VisualEffect _vfx;


    private void Start()
    {
        _vfx = _vfxOBJ.GetComponent<VisualEffect>();
    }
    public void PlayHitEffects()
    {
        if (_vfx != null)
        {
            _vfxOBJ.transform.position = transform.position;
            _vfx.Play();
        }
    }

    public void StopHitEffects()
    {
        if (_vfx != null)
            _vfx.Stop();
    }
}
