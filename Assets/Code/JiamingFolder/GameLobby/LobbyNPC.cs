using UnityEngine;

public class LobbyNPC : MonoBehaviour
{
    [SerializeField]
    private Dialogue dialouge;

    private bool dialougeStarted;

    private void Start()
    {
        InputManager inputManager = ServiceLocator.Instance.GetService<InputManager>();

        inputManager.OnClick += HandleClick;

    }
    private void OnTriggerEnter(Collider other)
    {
        if(dialougeStarted) return;

        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.StartDialogue(dialouge);
            dialougeStarted = true;
        }
    }

    private void HandleClick()
    {
        if (!dialougeStarted) return;

        if (DialogueManager.Instance.StillHaveDialogue())
        {
            DialogueManager.Instance.DisplayNextSentence();
        }
        else
        {
            dialougeStarted = false;
        }
        


    }
}
