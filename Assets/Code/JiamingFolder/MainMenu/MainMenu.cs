using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private string _mainMenuBGM = "MainMenuBGM";
    void Start()
    {
        ServiceLocator.Instance.GetService<AudioManager>().PlayBackgroundMusic(_mainMenuBGM);
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
