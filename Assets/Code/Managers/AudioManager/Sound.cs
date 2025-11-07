using UnityEngine;

[CreateAssetMenu(fileName = "NewSound", menuName = "Audio/Sound")]
public class Sound : ScriptableObject
{
    public AudioClip clip;
    public string audioName;

    [Range(0f, 1f)]
    public float volume;
}
