using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
      
        if (other.CompareTag("Runner") || other.CompareTag("Player"))
        {
            Debug.Log("You Win!");
            // add additional stuff later
        }
    }
}
