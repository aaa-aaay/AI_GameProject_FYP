using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class JiamingTagHandler : MonoBehaviour
{

    private float captureTimer = 0;
    private bool _stillCatching;
    [SerializeField] private GameObject timerGameObject;
    private TMP_Text timerText;
    [SerializeField] private ExitTrigger exitTrigger;
    private void Start()
    {
        captureTimer = 0;
        _stillCatching = true;
        timerText = timerGameObject.GetComponentInChildren<TMP_Text>();
    }
    public void HandleCatchFinish()
    {
        ServiceLocator.Instance.GetService<PostProcessingManager>().ShowTagNightEffects(true);
        exitTrigger.SetTimeTakenToCpature(captureTimer);
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
        timerText.text = captureTimer.ToString("F2");
        UpdateTimerUI(captureTimer);
    }
}
