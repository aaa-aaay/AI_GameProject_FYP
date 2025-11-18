using System.Runtime.CompilerServices;
using UnityEngine;

public class JiamingTagHandler : MonoBehaviour
{

    private float captureTimer = 0;
    private bool _stillCatching;

    private void Start()
    {
        captureTimer = 0;
        _stillCatching = true;
    }
    public void HandleCatchFinish()
    {
        ServiceLocator.Instance.GetService<PostProcessingManager>().ShowTagNightEffects(true);
        _stillCatching = false;
    }

    private void UpdateTimerUI(float timer)
    {

    }

    public void UpdateCaptureTimer()
    {
        if (!_stillCatching) return;
        captureTimer += Time.deltaTime;
        UpdateTimerUI(captureTimer);
    }
}
