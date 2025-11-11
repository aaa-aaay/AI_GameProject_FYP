using UnityEngine;
using System.Collections.Generic;

public class QuestionReceivingNPC : MonoBehaviour
{
    [SerializeField] private DialogueSpeechSO initialDialogue;

    private bool dialogueStarted;

    private DialogueManager dialogueManager;

    private LobbyPlayerMovement player;

    private void Start()
    {
        InputManager inputManager = ServiceLocator.Instance.GetService<InputManager>();

        inputManager.OnClick += HandleClick;

        dialogueManager = ServiceLocator.Instance.GetService<DialogueManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (dialogueStarted) return;

        if (other.CompareTag("Player"))
        {
            dialogueManager.StartDialogue(initialDialogue);
            dialogueStarted = true;
            player = other.gameObject.GetComponent<LobbyPlayerMovement>();
            player.ForceIdle(true);

            dialogueManager.onDialogueOver += handleDialogueOver;
        }
    }

    private void HandleClick()
    {
        if (!dialogueStarted) return;
        dialogueManager.DisplayNextSentence();
    }

    private void handleDialogueOver(int optionSelected)
    {
        Reset();
    }

    private void Reset()
    {
        player.ForceIdle(false);
        dialogueStarted = false;
        dialogueManager.onDialogueOver -= handleDialogueOver;
    }



}

