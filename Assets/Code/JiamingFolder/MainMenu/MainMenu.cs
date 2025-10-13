using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string _gameLobbySceneName = "GameLobby";

    void Start()
    {
        //play mainmenu bg music
    }

    public void GoGameLobby()
    {
        ServiceLocator.Instance.GetService<MySceneManager>().LoadScene(_gameLobbySceneName);
    }



}
