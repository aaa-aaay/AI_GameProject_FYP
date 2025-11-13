using System;
using System.Collections.Generic;
using Unity.InferenceEngine;
using UnityEngine;

public enum DifficultyLevel
{
    Easy = 0,
    Medium,
    Hard,
    Count
}

[Serializable]
public class DifficultyAI
{
    [SerializeField] public DifficultyLevel difficulty;
    [SerializeField] public ModelAsset ai;
}

[CreateAssetMenu(fileName = "DifficultySetting", menuName = "Scriptable Objects/DifficultySettings")]
public class DifficultySettings : ScriptableObject
{
    [SerializeField] public string model_name;

    [SerializeField] private List<DifficultyAI> difficulties;

    public ModelAsset GetModel(DifficultyLevel difficulty)
    {
        for (int i = 0; i < difficulties.Count; i++)
        {
            if (difficulties[i].difficulty == difficulty)
            {
                return difficulties[i].ai;
            }
        }

        return difficulties[0].ai;
    }
}
