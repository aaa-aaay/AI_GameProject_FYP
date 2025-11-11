using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialougeManager : MonoBehaviour, IGameService
{
    public static DialougeManager Instance { get; private set; }

    private Queue<string> sentences;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text sentenceText;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Animator animator;

    [SerializeField] private GameObject _buttonsPanel;
    [SerializeField] private List<GameObject> buttons;

    [SerializeField] private Image pfpImage;

    private bool haveDialouge;

    public event Action<int> onDialougeOver;


    private void OnEnable()
    {
        ServiceLocator.Instance.AddService(this, false);
        sentences = new Queue<string>();
        canvas.SetActive(false);
    }

    private void OnDisable()
    {
        //ServiceLocator.Instance.RemoveService<DialougeManager>(false);
    }


    public void StartDialogue(Dialogue dialouge)
    {
        sentences.Clear();
        canvas.SetActive(true);
        animator.SetBool("isOpen", true);
        haveDialouge = true;
        nameText.text = dialouge.name;
        pfpImage.sprite = dialouge.pfpSprite;
        _buttonsPanel.SetActive(false);


        if (dialouge.isMCQ) {
            _buttonsPanel.SetActive(true);
            sentenceText.text = "";
            int sentencesCount = 0;
            foreach(GameObject buttonGO in buttons)
            {

                
                if(sentencesCount >= dialouge.sentences.Count())
                {
                    buttonGO.SetActive(false);
                }
                else
                {
                    sentencesCount++;

                    TMP_Text displayText = buttonGO.GetComponentInChildren<TMP_Text>();
                    displayText.text = dialouge.sentences[sentencesCount - 1];
                }

            }
        }
        else
        {
            foreach (string sentence in dialouge.sentences)
            {
                sentences.Enqueue(sentence);

            }
            DisplayNextSentence();
        }


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
        onDialougeOver?.Invoke(0);

    }

    public void ReplyPressed(int replyNo)
    {
        haveDialouge = false;
        canvas.SetActive(false);
        animator.SetBool("isOpen", false);
        _buttonsPanel.SetActive(false);
        onDialougeOver?.Invoke(replyNo);
    }
    public bool StillHaveDialogue()
    {
        return haveDialouge;
    }
}
