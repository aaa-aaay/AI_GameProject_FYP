using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour, IGameService
{
    [SerializeField] private string _gameLobbyName;
    [SerializeField] private string _tutorialSceneName = "Tutorial";
    [SerializeField] private string _mainMenuSceneName = "MainMenu";


    private void OnEnable()
    {
        ServiceLocator.Instance.AddService(this, false);
    }
    private void OnDisable()
    {
        //ServiceLocator.Instance.RemoveService<MySceneManager>(false);
    }

    private void SetupWhenChangingScenes()
    {
        Time.timeScale = 1;
        ServiceLocator.Instance.GetService<AudioManager>().StopBGmWithFade();
        ServiceLocator.Instance.GetService<DialogueManager>().EndDialogue();
        ServiceLocator.Instance.GetService<UIManager>().DisableSettings(true);
    }


    public void LoadScene(string name)
    {
        SetupWhenChangingScenes();
        //handle transition animation here
        EventHolder.InvokeStartLoadScene(name);

    }

    public void LoadMiniGameWithTutorial(MiniGameSO minigame)
    {
        SetupWhenChangingScenes();
        ServiceLocator.Instance.GetService<UIManager>().SetMiniGameForTutorial(minigame);
        EventHolder.InvokeStartLoadScene(_tutorialSceneName);

    }

    public void restartScene()
    {
        SetupWhenChangingScenes();
        EventHolder.InvokeStartLoadScene(SceneManager.GetActiveScene().name);
    }
    public void GoBacktoGameLobby()
    {
        LoadScene(_gameLobbyName);
    }
    public void GoToMainMenu()
    {
        LoadScene(_mainMenuSceneName);
    }
}
