using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "MiniGameSO", menuName = "Scriptable Objects/MiniGameSO")]
public class MiniGameSO : ScriptableObject
{

    public string sceneName;
    public string gameName;


    [Header("For tutorial")]
    public List<VideoClip> tutorialVideoClips;
}
