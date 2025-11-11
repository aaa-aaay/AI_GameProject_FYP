using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialouge; 


    public void TriggerDialouge()
    {
        DialougeManager.Instance.StartDialogue( dialouge );
    }
}
