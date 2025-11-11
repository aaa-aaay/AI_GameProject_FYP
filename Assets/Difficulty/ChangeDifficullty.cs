using Unity.MLAgents;
using UnityEngine;

public class ChangeDifficullty : MonoBehaviour
{
    [SerializeField] private DifficultySettings settings;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        print("Difficulty is " + (DifficultyLevel)PlayerPrefs.GetInt("Difficulty"));
        GetComponent<Agent>().SetModel(settings.model_name, settings.GetModel((DifficultyLevel) PlayerPrefs.GetInt("Difficulty")));
    }
}
