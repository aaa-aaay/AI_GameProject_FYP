using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour, IGameService
{
    [SerializeField] private string _gameLobbyName;
    [SerializeField] private string _tutorialSceneName = "Tutorial";


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
        //handle transition animation here
        EventHolder.InvokeStartLoadScene(name);
        ServiceLocator.Instance.GetService<DialougeManager>().EndDialogue();
    }

    public void LoadMiniGameWithTutorial(MiniGameSO minigame)
    {
        ServiceLocator.Instance.GetService<UIManager>().SetMiniGameForTutorial(minigame);
        EventHolder.InvokeStartLoadScene(_tutorialSceneName);
        ServiceLocator.Instance.GetService<DialougeManager>().EndDialogue();
    }

    public void restartScene()
    {
        ServiceLocator.Instance.GetService<DialougeManager>().EndDialogue();
        EventHolder.InvokeStartLoadScene(SceneManager.GetActiveScene().name);
    }
    public void GoBacktoGameLobby()
    {
        LoadScene(_gameLobbyName);
    }
}
