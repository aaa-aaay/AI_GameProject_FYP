using UnityEngine;
using UnityEngine.VFX;

public class CarVFXController : MonoBehaviour
{
    [SerializeField] private VisualEffect[] _rightDriftVFXs;
    [SerializeField] private VisualEffect[] _leftDriftVFXs;
    private void Start()
    {
        foreach(VisualEffect vfx in _rightDriftVFXs) vfx.enabled = false;
        foreach (VisualEffect vfx in _leftDriftVFXs) vfx.enabled = false;
    }

    public void PlayDriftEffects(bool enable, float horizontalInput = 0)
    {

        if (!enable)
        {

            foreach (VisualEffect vfx in _rightDriftVFXs) vfx.enabled = false;
            foreach (VisualEffect vfx in _leftDriftVFXs) vfx.enabled = false;

            return;
        }
        else
        {
            Debug.Log(horizontalInput);
            if (horizontalInput <= -0.1f)
            {

                foreach (VisualEffect vfx in _rightDriftVFXs) vfx.enabled = false;
                foreach (VisualEffect vfx in _leftDriftVFXs) vfx.enabled = true;
                return;
            }
            if(horizontalInput >= 0.1f)
            {

                foreach (VisualEffect vfx in _rightDriftVFXs) vfx.enabled = true;
                foreach (VisualEffect vfx in _leftDriftVFXs) vfx.enabled = false;

                return;
            }
        }
           
    }
}
