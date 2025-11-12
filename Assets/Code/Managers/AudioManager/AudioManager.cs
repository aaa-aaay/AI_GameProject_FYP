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


    public void PlaySFX(string name, Vector3 position)
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
                    source.volume = sound.volume;
                    source.transform.position = position;
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
        float targetVolume = sound.volume;
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
        yield return new WaitForSeconds(source.clip.length);
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
        _backgroundMusicSource.volume = bgmVolume;
    }

    public void SetSFXVol(float sfxVolume)
    {
        foreach (var source in _audioSourcePool)
        {
            source.volume = sfxVolume;
        }
    }
}
