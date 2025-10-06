using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour, IGameService
{
    [SerializeField] private int gameLobbySceneNo = 0;

    private void OnEnable()
    {
        ServiceLocator.Instance.AddService(this, false);
    }
    private void OnDisable()
    {
        //ServiceLocator.Instance.RemoveService<MySceneManager>(false);
    }


    public void LoadScene(int sceneNo)
    {
        //handle transition animation here
        SceneManager.LoadScene(sceneNo);
    }

    public void restartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void GoBacktoGameLobby()
    {
        SceneManager.LoadScene(gameLobbySceneNo);
    }
}
