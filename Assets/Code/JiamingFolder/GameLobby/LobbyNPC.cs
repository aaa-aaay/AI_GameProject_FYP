using UnityEngine;

public class LobbyNPC : MonoBehaviour
{
    [SerializeField]
    private Dialogue dialouge;

    private bool dialougeStarted;

    private DialogueManager dialogueManager;

    private void Start()
    {
        InputManager inputManager = ServiceLocator.Instance.GetService<InputManager>();

        inputManager.OnClick += HandleClick;

        dialogueManager = ServiceLocator.Instance.GetService<DialogueManager>();

    }
    private void OnTriggerEnter(Collider other)
    {
        if(dialougeStarted) return;

        if (other.CompareTag("Player"))
        {
            dialogueManager.StartDialogue(dialouge);
            dialougeStarted = true;
        }
    }

    private void HandleClick()
    {
        if (!dialougeStarted) return;

        if (dialogueManager.StillHaveDialogue())
        {
            dialogueManager.DisplayNextSentence();
        }
        else
        {
            dialougeStarted = false;
        }
        


    }
}
