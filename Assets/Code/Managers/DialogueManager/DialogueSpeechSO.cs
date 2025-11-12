using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable, CreateAssetMenu(fileName = "Speech", menuName = "Dialogue/Speech")]
public class DialogueSpeechSO : ScriptableObject
{
    [SerializeField] private string speakerName;
    [SerializeField] private Sprite speakerSprite;
    [SerializeField, TextArea(3, 10)] private string speech;
    [SerializeField] private bool isMCQ;
    [SerializeField] private DialogueSpeechSO nextSpeech;
    [SerializeField] private DialogueChoice[] choices;

    public string SpeakerName => speakerName;
    public Sprite SpeakerSprite => speakerSprite;
    public string Speech => speech;
    public bool IsMCQ => isMCQ;
    public DialogueSpeechSO NextSpeech => nextSpeech;
    public DialogueChoice[] Choices => choices;

    #if UNITY_EDITOR
    // Choices cannot exceed maxChoices (UI limitations)
    private int maxChoices = 5;

    private void OnValidate()
    {
        if (choices != null && choices.Length > maxChoices)
        {
            Array.Resize(ref choices, maxChoices);
            Debug.LogWarning($"{name}: Choices truncated to a maximum of 5 elements.");
        }
    }
    #endif
}

[Serializable] public struct DialogueChoice
{
    [SerializeField, TextArea(1, 10)] private string optionText;
    [SerializeField] private DialogueSpeechSO nextSpeech;

    public string OptionText => optionText;
    public DialogueSpeechSO NextSpeech => nextSpeech;
}

// Show/Hide variables for DialogueSpeechSO
#if UNITY_EDITOR
[CustomEditor(typeof(DialogueSpeechSO))]
public class DialogueSpeechSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("speakerName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("speakerSprite"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("speech"));

        SerializedProperty isMCQProp = serializedObject.FindProperty("isMCQ");
        EditorGUILayout.PropertyField(isMCQProp);

        if (isMCQProp.boolValue) // isMCQ == true
            EditorGUILayout.PropertyField(serializedObject.FindProperty("choices"), true);
        else // isMCQ == false
            EditorGUILayout.PropertyField(serializedObject.FindProperty("nextSpeech"));

        serializedObject.ApplyModifiedProperties();
    }
}
#endif