using System;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "NPC", menuName = "Dialogue/NPC Info")]
public class DialogueNPCSO : ScriptableObject
{
    [SerializeField] private string npcName;
    [SerializeField] private Sprite npcSprite;

    public string NpcName => npcName;
    public Sprite NpcSprite => npcSprite;
}
