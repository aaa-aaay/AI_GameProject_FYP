using System.Collections;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void StartLoad(string scene_name)
    {
        gameObject.SetActive(true);
        StartCoroutine(AsynchronousLoad(scene_name));
        //SceneManager.LoadSceneAsync(scene_name);
    }

    IEnumerator AsynchronousLoad(string scene)
    {
        yield return null;
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);

        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // [0, 0.9] &gt; [0, 1]
            float value = Mathf.Clamp01(operation.progress / 0.9f);

            slider.value = value;
            text.text = (value * 100).ToString() + "%";

            if (value >= 1)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
    }
}
