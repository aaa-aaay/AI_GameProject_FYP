using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "MiniGameSO", menuName = "Scriptable Objects/MiniGameSO")]
public class MiniGameSO : ScriptableObject
{
    [Header("General")]
    public string sceneName;
    public string gameName;

    [Header("For Level Select UI")]
    public Sprite levelSelectPanelSprite;

    [Header("For mini game Over UI")]
    [TextArea(2, 10)] public string[] starConditions;
    [TextArea(2, 10)] public string levelFailedText;


    [Header("For tutorial")]
    public List<VideoClip> tutorialVideoClips;
}
