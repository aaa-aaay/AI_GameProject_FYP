using UnityEngine;

public class LobbyNPC : MonoBehaviour
{
    [SerializeField]
    private Dialogue dialouge;

    private bool dialougeStarted;

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
        if(dialougeStarted) return;

        if (other.CompareTag("Player"))
        {
            dialogueManager.StartDialogue(dialouge);
            dialougeStarted = true;
            player = other.gameObject.GetComponent<LobbyPlayerMovement>();
            player.ForceIdle(true);
        }
    }

    private void HandleClick()
    {
        if (!dialougeStarted) return;

        if (dialogueManager.StillHaveDialogue())
        {
            dialogueManager.DisplayNextSentence();
        }
    }

    private void Update()
    {
        if (!dialougeStarted) return;

        if (!dialogueManager.StillHaveDialogue())
        {
            player.ForceIdle(false);
            dialougeStarted = false;
        }
    }
}
