using DG.Tweening;
using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class CountDownTimer : MonoBehaviour
{
    [SerializeField] private int _countDownStartNumber;
    [SerializeField] private string _endText = "GO!";
    [SerializeField] private TMP_Text _countDownText;
    [SerializeField] private GameObject _countDownCanvasGO;

    private CanvasGroup _canvasGroup;

    private int _countDownCount;

    private void Start()
    {
        _canvasGroup = _countDownCanvasGO.GetComponent<CanvasGroup>();
    }
    public void StartCountDown(float _startTimerAfterSeconds = 0)
    {
        StartCoroutine(StartCountDownAfterWait(_startTimerAfterSeconds));
    }

    private IEnumerator CountDown()
    {

        if(_countDownCount == 0)
        {
            _countDownText.text = _endText;
            _canvasGroup.DOFade(1, 0.3f).SetUpdate(true);
            _countDownText.rectTransform.DOScale(Vector3.one, .3f).SetUpdate(true).OnComplete(AnimationComplete);
            yield return new WaitForSecondsRealtime(1f);
            _countDownCanvasGO.SetActive(false);
            Time.timeScale = 1f; //reEnable the game;
            yield break;
        }

        _countDownText.text = _countDownCount.ToString();
        yield return null;
        _canvasGroup.DOFade(1,0.3f).SetUpdate(true); //animate even when paused

        _countDownText.rectTransform.DOScale(Vector3.one,.3f).SetUpdate(true).OnComplete(AnimationComplete);


        yield return new WaitForSecondsRealtime(1f);
        if(_countDownCount > 0)
        {
            _countDownCount--;
            StartCoroutine(CountDown());
        }
    }

    private void AnimationComplete()
    {
        _canvasGroup.DOFade(0, 0.3f).SetUpdate(true).SetDelay(0.3f);
        _countDownText.rectTransform.DOScale(Vector3.zero, 0.3f).SetUpdate(true).SetDelay(.3f);

    }

    private IEnumerator StartCountDownAfterWait(float time)
    {
        yield return new WaitForSeconds(time);

        _countDownCount = _countDownStartNumber;
        _countDownCanvasGO.SetActive(true);
        Time.timeScale = 0f; //pause the game
        StartCoroutine(CountDown());
    }
}
