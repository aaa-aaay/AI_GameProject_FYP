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


    public void LoadScene(string name)
    {
        Time.timeScale = 1;
        //handle transition animation here
        EventHolder.InvokeStartLoadScene(name);
        ServiceLocator.Instance.GetService<DialogueManager>().EndDialogue();
    }

    public void LoadMiniGameWithTutorial(MiniGameSO minigame)
    {
        Time.timeScale = 1;
        ServiceLocator.Instance.GetService<UIManager>().SetMiniGameForTutorial(minigame);
        EventHolder.InvokeStartLoadScene(_tutorialSceneName);
        ServiceLocator.Instance.GetService<DialogueManager>().EndDialogue();
    }

    public void restartScene()
    {
        Time.timeScale = 1;
        ServiceLocator.Instance.GetService<DialogueManager>().EndDialogue();
        EventHolder.InvokeStartLoadScene(SceneManager.GetActiveScene().name);
    }
    public void GoBacktoGameLobby()
    {
        Time.timeScale = 1;
        LoadScene(_gameLobbyName);
    }
    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        LoadScene(_mainMenuSceneName);
    }
}
