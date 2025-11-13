using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
  
    void Start()
    {
        AudioManager audioManager = ServiceLocator.Instance.GetService<AudioManager>();
        audioManager.PlayBackgroundMusic("BGMTest");
    }

}
