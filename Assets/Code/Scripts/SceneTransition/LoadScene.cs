using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private string scene_name;

    public void ChangeScene()
    {
        EventHolder.InvokeStartLoadScene(scene_name);
    }
}
