using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour, IGameService
{
    [SerializeField] private string _gameLobbyName;


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
    }

    public void restartScene()
    {
        //Debug.Log(SceneManager.GetActiveScene().name);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        EventHolder.InvokeStartLoadScene(SceneManager.GetActiveScene().name);
    }
    public void GoBacktoGameLobby()
    {
        EventHolder.InvokeStartLoadScene(_gameLobbyName);
    }
}
