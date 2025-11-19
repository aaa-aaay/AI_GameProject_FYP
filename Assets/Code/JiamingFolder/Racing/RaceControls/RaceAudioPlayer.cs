using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RaceAudioPlayer : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private AudioSource _drftingSound;
    [SerializeField] private AudioSource _drivingSound;

    [SerializeField] private  AudioSource[] _cheeringAudioSource;

    [SerializeField] private float maxVolume = 0.9f;
    [SerializeField] private float minVolume = 0.0f;
    [SerializeField] private float maxSpeedForVolume = 20f;

    [Header("Drift SFX Fade")]
    [SerializeField] private float driftFadeTime = 0.3f;

    private Coroutine driftRoutine;
    private float audiovol;
    private float cheeringAudioSourceVol;


    private void Start()
    {
        cheeringAudioSourceVol = _cheeringAudioSource.First().volume;
    }
    private void Update()
    {

        audiovol = ServiceLocator.Instance.GetService<AudioManager>().GetSFXVol();
        PlayDrivingSound();



        foreach(AudioSource source in _cheeringAudioSource)
        {
            source.volume = cheeringAudioSourceVol * audiovol;
        }
    }

    public void PlayDrivingSound()
    {
        float localForwardSpeed =  Mathf.Abs(transform.InverseTransformDirection(_rb.linearVelocity).z);
        float forwardSpeed = Mathf.Max(0f, localForwardSpeed);
        float t = Mathf.InverseLerp(0, maxSpeedForVolume, Mathf.Abs(forwardSpeed));

        t = Mathf.SmoothStep(0, 1, t);
        _drivingSound.volume = Mathf.Lerp(minVolume, maxVolume * audiovol, t) ;
    }

    public void StartDriftSFX()
    {
        StartDriftFade(audiovol);
    }

    public void EndDriftSFX()
    {
        StartDriftFade(0f);
    }

    private void StartDriftFade(float targetVolume)
    {
        if (driftRoutine != null)
            StopCoroutine(driftRoutine);

        driftRoutine = StartCoroutine(FadeDriftVolume(targetVolume));
    }

    private IEnumerator FadeDriftVolume(float target)
    {
        float start = _drftingSound.volume;
        float time = 0f;

        // Start playing if fading in
        if (target > 0f && !_drftingSound.isPlaying)
            _drftingSound.Play();

        while (time < driftFadeTime)
        {
            time += Time.deltaTime;
            float t = time / driftFadeTime;

            _drftingSound.volume = Mathf.Lerp(start, target, t);
            yield return null;
        }

        _drftingSound.volume = target;

        if (Mathf.Approximately(target, 0f))
            _drftingSound.Stop();
    }
}
