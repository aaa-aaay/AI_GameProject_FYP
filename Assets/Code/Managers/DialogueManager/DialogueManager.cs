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

    private DialogueSpeechSO currentSentence;

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
            DisplaySentence();
            return;
        }

        if (!currentSentence.IsMCQ) currentSentence = currentSentence.NextSpeech; // Display next sentence if not MCQ
        else if (choice < 0) return; // Ignore if MCQ and no choice made
        else currentSentence = currentSentence.Choices[choice].NextSpeech;

        if (!currentSentence)
        {
            EndDialogue();
            return;
        }

        if (currentSentence.IsMCQ)
            DisplayChoices();
        else
            DisplaySentence();
    }

    private void DisplaySentence()
    {
        speechText.gameObject.SetActive(true);
        buttonPanel.gameObject.SetActive(false);

        speakerNameText.text = currentSentence.SpeakerName;
        speakerImage.sprite = currentSentence.SpeakerSprite;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentSentence.Speech));
    }

    private void DisplayChoices()
    {
        speechText.gameObject.SetActive(false);
        buttonPanel.gameObject.SetActive(true);
        foreach (RectTransform t in buttons) t.gameObject.SetActive(false);

        speakerNameText.text = currentSentence.SpeakerName;
        speakerImage.sprite = currentSentence.SpeakerSprite;

        int i = 0;
        foreach (DialogueChoice c in currentSentence.Choices)
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
