using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionPlayer : MonoBehaviour
{
    [SerializeField] private LoadingBar loading_bar;
    [SerializeField] private bool _playCountDownAfterCutScene = false;

    private Animator animator;

    private string load_scene;

    void Start()
    {
        EventHolder.StartLoadScene += OnStartLoadScene;
        animator = GetComponent<Animator>();
    }



    private void OnDestroy()
    {
        EventHolder.StartLoadScene -= OnStartLoadScene;
    }

    public void OnStartLoadScene(string scene_name)
    {
        load_scene = scene_name;
        PlayStartLoadAnimation();
    }

    public void StartLoad()
    {
        loading_bar.StartLoad(load_scene);
    }
    public void StartCountDownUI()
    {
        if(_playCountDownAfterCutScene)
        ServiceLocator.Instance.GetService<UIManager>().StartCountDownTimer();
        else Time.timeScale = 1f;

    }

    private void PlayStartLoadAnimation()
    {
        animator.ResetTrigger("EndLoad");
        animator.SetTrigger("StartLoad");
    }

    private void PlayEndLoadAnimation()
    {
        animator.ResetTrigger("StartLoad");
        animator.SetTrigger("EndLoad");
    }
}
