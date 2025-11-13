using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
 
    private AudioManager _audioManager;
    [SerializeField] private string _bgmName = string.Empty;
    void Start()
    {
        _audioManager = ServiceLocator.Instance.GetService<AudioManager>();
        if(_bgmName != string.Empty) _audioManager.PlayBackgroundMusic(_bgmName);
    }

    public void PlaySFX(string sfxName)
    {
        _audioManager.PlaySFX(sfxName, Camera.main.transform.position);
    }

}
