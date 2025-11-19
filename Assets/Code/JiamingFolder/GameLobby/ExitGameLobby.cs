using UnityEngine;

public class ExitGameLobby : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<LobbyPlayerMovement>().enabled = false;
            ServiceLocator.Instance.GetService<MySceneManager>().LoadScene("MainMenu");
        }
    }
}
