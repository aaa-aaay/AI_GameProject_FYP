using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class JiamingTagHandler : MonoBehaviour
{

    private float captureTimer = 0;
    private bool _stillCatching;
    [SerializeField] private GameObject timerGameObject;
    private TMP_Text timerText;

    private void Start()
    {
        captureTimer = 0;
        _stillCatching = true;
        timerText = timerGameObject.GetComponentInChildren<TMP_Text>();
    }
    public void HandleCatchFinish()
    {
        ServiceLocator.Instance.GetService<PostProcessingManager>().ShowTagNightEffects(true);
        _stillCatching = false;
        timerGameObject.SetActive(false);
    }

    private void UpdateTimerUI(float timer)
    {

    }

    public void UpdateCaptureTimer()
    {
        if (!_stillCatching) return;
        captureTimer += Time.deltaTime;
        timerText.text = captureTimer.ToString();
        UpdateTimerUI(captureTimer);
    }
}
