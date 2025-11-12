using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void GoGameLobby()
    {
        ServiceLocator.Instance.GetService<MySceneManager>().GoBacktoGameLobby();
    }

    public void OpenSettings()
    {
        ServiceLocator.Instance.GetService<UIManager>().ToggleSettingsPage();
    }



}
