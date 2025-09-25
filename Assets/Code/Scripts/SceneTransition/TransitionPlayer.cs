using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionPlayer : MonoBehaviour
{
    [SerializeField] private LoadingBar loading_bar;

    private Animator animator;

    private string load_scene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();

        EventHolder.StartLoadScene += OnStartLoadScene;
    }

    private void OnDestroy()
    {
        EventHolder.StartLoadScene -= OnStartLoadScene;

    }

    public void OnStartLoadScene(string scene_name)
    {
        if (gameObject.scene.name != scene_name)
        {
            load_scene = scene_name;
             PlayStartLoadAnimation();
        }
    }

    public void StartLoad()
    {
        loading_bar.StartLoad(load_scene);
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
