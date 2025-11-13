using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class SceneChangeData
{
    public string scene_name;
    public Color scene_color;
    public Sprite scene_sprite;
}

public class LoadingBar : MonoBehaviour
{
    [SerializeField] private Image loading_bg;
    [SerializeField] private Image loading_game;

    [SerializeField] private List<SceneChangeData> scene_changes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void StartLoad(string scene_name)
    {
        gameObject.SetActive(true);

        loading_bg.gameObject.SetActive(true);

        if (scene_name == "Tutorial")
        {
            for (int i = 0; i < scene_changes.Count; i++)
            {
                string actual_scene_name = ServiceLocator.Instance.GetService<UIManager>().GetMiniGameForTutorial().sceneName;
                if (scene_changes[i].scene_name == actual_scene_name)
                {
                    loading_game.gameObject.SetActive(true);
                    loading_bg.color = scene_changes[i].scene_color;
                    loading_game.sprite = scene_changes[i].scene_sprite;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < scene_changes.Count; i++)
            {
                if (scene_changes[i].scene_name == scene_name)
                {
                    loading_game.gameObject.SetActive(true);
                    loading_bg.color = scene_changes[i].scene_color;
                    loading_game.sprite = scene_changes[i].scene_sprite;
                    break;
                }
            }
        }

        StartCoroutine(AsynchronousLoad(scene_name));
        //SceneManager.LoadSceneAsync(scene_name);
    }

    IEnumerator AsynchronousLoad(string scene)
    {
        yield return null;
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        Debug.Log(scene);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            print(operation.progress);
            if (operation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(2.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
    }
}
