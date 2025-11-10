using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        //play mainmenu bg music
    }

    public void GoGameLobby()
    {
        ServiceLocator.Instance.GetService<MySceneManager>().GoBacktoGameLobby();
    }

    public void OpenSettings()
    {
        ServiceLocator.Instance.GetService<UIManager>().ToggleSettingsPage();
    }



}
