using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IGCC
{


    [CreateAssetMenu(fileName = "NewSound", menuName = "Audio/Sound")]
    public class Sound : ScriptableObject
    {
        public AudioClip clip;
        public string audioName;

        [Range(0f, 1f)]
        public float volume;
    }
}
