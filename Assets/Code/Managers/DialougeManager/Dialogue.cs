using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Dialogue
{
    public string name;
    public Sprite pfpSprite;
    [TextArea(3,10)]public string[] sentences;
    public bool isMCQ;
}