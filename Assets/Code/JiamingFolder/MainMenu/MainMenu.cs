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
        Application.OpenURL("https://docs.google.com/presentation/d/1XMuK7ppaOS9fbP7C1iPwgGWCvyIoHoxQr-LBrRrmE6E/edit?usp=sharing");
    }



}
