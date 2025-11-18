using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour, IGameService
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private Image speakerImage;
    [SerializeField] private TMP_Text speechText;

    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform buttonPanel;
    [SerializeField] private RectTransform[] buttons;

    [SerializeField] private Animator animator;

    private DialogueOption currentSentence;

    private bool haveDialogue;

    public event Action<int> onDialogueOver;

    private void OnEnable()
    {
        ServiceLocator.Instance.AddService(this, false);
        canvas.gameObject.SetActive(false);
    }

    public void StartDialogue(DialogueSpeechSO speech)
    {
        canvas.gameObject.SetActive(true);
        buttonPanel.gameObject.SetActive(false);
        animator.SetBool("isOpen", true);
        haveDialogue = true;

        DisplayNextSentence(-1, speech);
    }

    public void DisplayNextSentence(int choice = -1, DialogueSpeechSO speech = null)
    {
        if (speech) // Initial speech
        {
            currentSentence = speech;
            DisplaySentence(speech);
            return;
        }

        if (currentSentence is DialogueSpeechSO speechNodeBefore)
        {
            if (!speechNodeBefore.IsMCQ) currentSentence = speechNodeBefore.NextSpeech; // Display next sentence if not MCQ
            else if (choice < 0) return; // Ignore if MCQ and no choice made
            else currentSentence = speechNodeBefore.Choices[choice].NextSpeech;
        }
        else if (currentSentence is DialogueActionSO actionNodeBefore)
        {
            currentSentence = actionNodeBefore.NextSpeech;
        }

        if (!currentSentence)
        {
            EndDialogue();
            return;
        }

        if (currentSentence is DialogueSpeechSO speechNodeAfter)
        {
            if (speechNodeAfter.IsMCQ)
                DisplayChoices(speechNodeAfter);
            else
                DisplaySentence(speechNodeAfter);
        }
        else if (currentSentence is DialogueActionSO actionNodeAfter)
        {
            if (actionNodeAfter.ActionType == dialogueActionType.Scene)
            {
                ServiceLocator.Instance.GetService<MySceneManager>().LoadScene(actionNodeAfter.SceneName);
                EndDialogue();
                return;
            }
            else if (actionNodeAfter.ActionType == dialogueActionType.MinigameScene)
            {
                ServiceLocator.Instance.GetService<MySceneManager>().LoadMiniGameWithTutorial(actionNodeAfter.Minigame);
                EndDialogue();
                return;
            }
        }
    }

    private void DisplaySentence(DialogueSpeechSO speechNode)
    {
        speechText.gameObject.SetActive(true);
        buttonPanel.gameObject.SetActive(false);

        speakerNameText.text = speechNode.NpcInfo.NpcName;
        speakerImage.sprite = speechNode.NpcInfo.NpcSprite;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(speechNode.Speech));
    }

    private void DisplayChoices(DialogueSpeechSO speechNode)
    {
        speechText.gameObject.SetActive(false);
        buttonPanel.gameObject.SetActive(true);
        foreach (RectTransform t in buttons) t.gameObject.SetActive(false);

        speakerNameText.text = speechNode.NpcInfo.NpcName;
        speakerImage.sprite = speechNode.NpcInfo.NpcSprite;

        int i = 0;
        foreach (DialogueChoice c in speechNode.Choices)
        {
            buttons[i].gameObject.SetActive(true);
            buttons[i].GetComponentInChildren<TMP_Text>().text = c.OptionText;
            i++;
        }
    }

    private IEnumerator TypeSentence(string sentence)
    {
        speechText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            speechText.text += letter;
            yield return null;
        }
    }

    public void EndDialogue()
    {
        haveDialogue = false;
        canvas.gameObject.SetActive(false);
        animator.SetBool("isOpen", false);
        onDialogueOver?.Invoke(0);
    }

    public void ReplyPressed(int replyNo)
    {
        DisplayNextSentence(replyNo - 1);
    }
}

public class DialogueOption : ScriptableObject
{
}