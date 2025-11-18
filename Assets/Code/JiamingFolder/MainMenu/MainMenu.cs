using UnityEngine;

public class MainMenu : MonoBehaviour
{

    private void Start()
    {

    }
    public void GoGameLobby()
    {
        ServiceLocator.Instance.GetService<MySceneManager>().GoBacktoGameLobby();
    }

    public void OpenSettings()
    {
        ServiceLocator.Instance.GetService<UIManager>().ToggleSettingsPage();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenCredits()
    {

    }



}
