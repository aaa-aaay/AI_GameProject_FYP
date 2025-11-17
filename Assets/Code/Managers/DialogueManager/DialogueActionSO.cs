using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable, CreateAssetMenu(fileName = "Action", menuName = "Dialogue/Action")]
public class DialogueActionSO : DialogueOption
{
    [SerializeField] private dialogueActionType actionType = dialogueActionType.Scene;
    [SerializeField] private string sceneName;
    [SerializeField] private MiniGameSO minigame;
    [SerializeField] private DialogueOption nextSpeech;

    public dialogueActionType ActionType => actionType;
    public string SceneName => sceneName;
    public MiniGameSO Minigame => minigame;
    public DialogueOption NextSpeech => nextSpeech;
}

public enum dialogueActionType
{
    Scene,
    MinigameScene
}

// Show/Hide variables for DialogueOption
#if UNITY_EDITOR
[CustomEditor(typeof(DialogueActionSO))]
public class DialogueActionSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty actionType = serializedObject.FindProperty("actionType");
        EditorGUILayout.PropertyField(actionType);


        if ((dialogueActionType)actionType.enumValueIndex == dialogueActionType.Scene)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sceneName"));
        }
        else if ((dialogueActionType)actionType.enumValueIndex == dialogueActionType.MinigameScene)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("minigame"));
        }
        else
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("nextSpeech"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif