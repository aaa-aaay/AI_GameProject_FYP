using UnityEngine;
using System.Collections.Generic;

public class QuestionRecivingNPC : MonoBehaviour
{
    [SerializeField] private Dialogue _initaldialouge;

    [SerializeField] private Dialogue _question;

    private bool dialougeStarted;
    private bool _questionReplied;

    private DialougeManager dialogueManager;

    private LobbyPlayerMovement player;

    [SerializeField] private List<Dialogue> _replyList;


    private bool _inQuesiton;

    private void Start()
    {
        InputManager inputManager = ServiceLocator.Instance.GetService<InputManager>();

        inputManager.OnClick += HandleClick;

        dialogueManager = ServiceLocator.Instance.GetService<DialougeManager>();

        _inQuesiton = false;

    }
    private void OnTriggerEnter(Collider other)
    {
        if (dialougeStarted) return;

        if (other.CompareTag("Player"))
        {
            dialogueManager.StartDialogue(_initaldialouge);
            dialougeStarted = true;
            player = other.gameObject.GetComponent<LobbyPlayerMovement>();
            player.ForceIdle(true);

            dialogueManager.onDialougeOver += handleDialougeOver;
        }
    }

    private void HandleClick()
    {
        if (!dialougeStarted) return;
        if (_inQuesiton) return;

        if (dialogueManager.StillHaveDialogue())
        {
            dialogueManager.DisplayNextSentence();
        }
    }

    //private void Update()
    //{
    //    if (!dialougeStarted) return;

    //    if (!dialogueManager.StillHaveDialogue())
    //    {
    //        player.ForceIdle(false);
    //        dialougeStarted = false;
    //    }
    //}
    
    private void handleDialougeOver(int optionSelected)
    {
        if (_questionReplied)
        {
            Reset();
            return;
        }
        Debug.Log("Option Seleected:   " + optionSelected);
        if (optionSelected == 0) //0 means not a question
        {
            dialogueManager.StartDialogue(_question); // player asks a question
            _inQuesiton = true;

        }
        else if (optionSelected - 1 <= _replyList.Count) {
            dialogueManager.StartDialogue(_replyList[optionSelected - 1]);
            _inQuesiton = false;
            _questionReplied = true;

        } // player replies
            
    }

    private void Reset()
    {
        player.ForceIdle(false);
        _questionReplied = false;
        _inQuesiton = false;
        dialougeStarted = false;
        dialogueManager.onDialougeOver -= handleDialougeOver;

    }



}

