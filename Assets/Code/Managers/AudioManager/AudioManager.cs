using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour, IGameService
{
    // Pool for AudioSources
    private Queue<AudioSource> _audioSourcePool = new Queue<AudioSource>();

    // AudioSource just for background music
    private AudioSource _backgroundMusicSource;
    private Dictionary<string, Sound> _soundDictionary = new Dictionary<string, Sound>();


    [SerializeField] 
    private int _poolSize = 10;

    [SerializeField]
    private float _backgroundMusicFadeTime;


    public Sound[] sounds;

    [Range(0, 1)] private float sfxVol = 1;
    [Range(0, 1)] private float bgmVol = 1;
    [Range(0, 1)] private float bgmSoundVol;



    private void OnEnable()
    {
        ServiceLocator.Instance.AddService(this, false);

        // Init the audio source pool
        for (int i = 0; i < _poolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.spatialBlend = 1.0f;
            source.rolloffMode = AudioRolloffMode.Logarithmic;
            source.minDistance = 1.0f;
            source.maxDistance = 30.0f;
            _audioSourcePool.Enqueue(source);
        }

        sfxVol = 1;
        bgmVol = 1;
        bgmSoundVol = 1;

        LoadAllSounds();

        _backgroundMusicSource = gameObject.AddComponent<AudioSource>();
        _backgroundMusicSource.loop = true;
        _backgroundMusicSource.spatialBlend = 0.0f;
        _backgroundMusicSource.playOnAwake = false;
    }

    private void OnDisable()
    {
        //ServiceLocator.Instance.RemoveService<AudioManager>(false);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += SetDefaultLocation;
    }

    private void SetDefaultLocation(Scene scene, LoadSceneMode mode)
    {

    }

    public void PlaySFXWithOutPos(string name)
    {
        PlaySFX(name);
    }
    public void PlaySFX(string name, Vector3? position = null)
    {

        foreach (Sound sound in sounds)
        {
            if (sound.audioName == name)
            {
                // Get an available AudioSource from the pool
                if (_audioSourcePool.Count > 0)
                {
                    AudioSource source = _audioSourcePool.Dequeue();
                    source.clip = sound.clip;
                    source.volume = sfxVol * sound.volume;

                    if (position.HasValue)
                    {
                        source.spatialBlend = 1.0f;
                        source.transform.position = position.Value;
                    }
                    else
                    {
                        source.spatialBlend = 0.0f;
                    }

                    source.Play();
                    StartCoroutine(ReturnToPoolAfterPlayback(source));
                }
                else
                {
                    Debug.Log("No audio sources !!!!");
                }
                return;
            }
        }
    }

    public void PlayBackgroundMusic(string name)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.audioName == name)
            {
                StartCoroutine(FadeBackgroundMusic(sound, _backgroundMusicFadeTime));
                return;
            }
        }
    }

    public void StopBGmWithFade()
    {
        if (_backgroundMusicSource.clip != null && _backgroundMusicSource.isPlaying)
        {
            StartCoroutine(FadeOutBGM(_backgroundMusicFadeTime));
        }
    }


    private IEnumerator FadeOutBGM(float duration)
    {
        float startVolume = _backgroundMusicSource.volume;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            _backgroundMusicSource.volume = Mathf.Lerp(startVolume, 0f, t);

            yield return null;
        }

        _backgroundMusicSource.Stop();
        _backgroundMusicSource.clip = null;
        _backgroundMusicSource.volume = startVolume; // reset for next BGM play
    }

    private IEnumerator FadeBackgroundMusic(Sound sound, float duration)
    {
        float fadeScale = 1 / duration;
        if (_backgroundMusicSource.clip)
        {
            float fadeOutTime = 0;
            float startVolume = _backgroundMusicSource.volume;
            while (fadeOutTime < 1)
            {
                fadeOutTime += Time.deltaTime * fadeScale;
                _backgroundMusicSource.volume = Mathf.Lerp(startVolume, 0, fadeOutTime);
                yield return null;
            }
        }

        float fadeInTime = 0;
        Debug.Log("Volume:" + bgmVol);
        bgmSoundVol = sound.volume;
        float targetVolume = bgmVol * sound.volume;
        _backgroundMusicSource.clip = sound.clip;
        _backgroundMusicSource.Play();
        while (fadeInTime < 1)
        {
            fadeInTime += Time.deltaTime * fadeScale;
            _backgroundMusicSource.volume = Mathf.Lerp(0, targetVolume, fadeInTime);
            yield return null;
        }
    }

    private IEnumerator ReturnToPoolAfterPlayback(AudioSource source)
    {
        yield return new WaitForSecondsRealtime(source.clip.length);
        source.Stop();
        source.clip = null;
        _audioSourcePool.Enqueue(source);
    }

    private void LoadAllSounds()
    {
        sounds = Resources.LoadAll<Sound>("AudioSO"); // make sure Sound SO is in this directory
        foreach (Sound sound in sounds)
        {
            if (!_soundDictionary.ContainsKey(sound.name))
            {
                _soundDictionary.Add(sound.name, sound);
            }
            else
            {
                Debug.LogWarning($"Duplicate sound name found: {sound.name}. Skipping.");
            }
        }
    }


    public void SetBGMVol(float bgmVolume)
    {
        _backgroundMusicSource.volume = bgmVolume * bgmSoundVol;
        bgmVol = bgmVolume;
    }

    public void SetSFXVol(float sfxVolume)
    {
        foreach (var source in _audioSourcePool)
        {
            source.volume = sfxVolume;
        }
        sfxVol = sfxVolume;
    }

    public float GetBGMVol()
    {
        return bgmVol;
    }
    public float GetSFXVol() { 
    
        return sfxVol;
    }
}
