using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player collides
        if (other.CompareTag("Runner") || other.CompareTag("Player"))
        {
            if(other.gameObject.GetComponent<MiniGameOverHandler>() != null)
            {
                other.gameObject.GetComponent<MiniGameOverHandler>().HandleGameOver(true, 1, 3);
            }
            else
            {
                Debug.Log("faled");
            }
            
            // Later, you can trigger win UI or end the round here
        }
    }
}
