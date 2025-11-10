using UnityEngine;

public class ExitGameLobby : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ServiceLocator.Instance.GetService<MySceneManager>().LoadScene("MainMenu");
        }
    }
}
