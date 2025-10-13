using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour, IGameService
{
    public static DialogueManager Instance { get; private set; }

    private Queue<string> sentences;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text sentenceText;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Animator animator;

    [SerializeField] private Image pfpImage;

    private bool haveDialouge;


    private void OnEnable()
    {
        ServiceLocator.Instance.AddService(this, false);
        sentences = new Queue<string>();
        canvas.SetActive(false);
    }

    private void OnDisable()
    {
        //ServiceLocator.Instance.RemoveService<DialogueManager>(false);
    }


    public void StartDialogue(Dialogue dialouge)
    {
        sentences.Clear();
        canvas.SetActive(true);
        animator.SetBool("isOpen", true);
        haveDialouge = true;
        nameText.text = dialouge.name;
        pfpImage.sprite = dialouge.pfpSprite;

        foreach (string sentence in dialouge.sentences) { 
            sentences.Enqueue(sentence);
        
        
        
        }
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0) {
            EndDialogue();
            return;
        
        
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        sentenceText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            sentenceText.text += letter;
            yield return null;


        }
    }

    public void EndDialogue()
    {
        haveDialouge = false;
        canvas.SetActive(false);
        animator.SetBool("isOpen", false);

    }
    public bool StillHaveDialogue()
    {
        return haveDialouge;
    }
}
