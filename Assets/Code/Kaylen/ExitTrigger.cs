using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player collides
        if (other.CompareTag("Runner") || other.CompareTag("Player"))
        {
            Debug.Log("You Win!");
            // Later, you can trigger win UI or end the round here
        }
    }
}
