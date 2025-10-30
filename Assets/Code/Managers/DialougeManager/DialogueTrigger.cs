using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialouge; 


    public void TriggerDialouge()
    {
        DialogueManager.Instance.StartDialogue( dialouge );
    }
}
