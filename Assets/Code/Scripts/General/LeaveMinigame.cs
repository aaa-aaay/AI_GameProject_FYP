using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaveMinigame : MonoBehaviour
{
    public void EnableButton()
    {
        string current_scene = SceneManager.GetActiveScene().name;
        if (current_scene == "MainMenu" || current_scene == "GameHub")
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    public void Return()
    {
        ServiceLocator.Instance.GetService<MySceneManager>().LoadScene("GameHub");
    }
}
