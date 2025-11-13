using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionPlayer : MonoBehaviour
{
    [SerializeField] private LoadingBar loading_bar;
    [SerializeField] private bool _playCountDown = false;
    [SerializeField] private GameObject _hider;
    private Animator animator;

    private string load_scene;
    private void Awake()
    {
        _hider.SetActive(true);
    }
    void Start()
    {

        EventHolder.StartLoadScene += OnStartLoadScene;
        animator = GetComponent<Animator>();
        StartCoroutine(Delay() );
    }

    IEnumerator Delay()
    {
        yield return null;
        yield return null;
        animator.enabled = true;
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
        PlayEndLoadAnimation();
    }
    public void StartCountDownUI()
    {
        if(_playCountDown)
        ServiceLocator.Instance.GetService<UIManager>().StartCountDownTimer();
    }

    private void PlayStartLoadAnimation()
    {
        if (!animator.GetBool("StartLoad"))
        {
            animator.SetBool("EndLoad", false);
            animator.SetBool("EndSceneLoad", false);
            animator.SetBool("StartLoad", true);
        }
    }

    private void PlayEndLoadAnimation()
    {
        if (!animator.GetBool("EndLoad"))
        {
            animator.SetBool("StartLoad", false);
            animator.SetBool("EndSceneLoad", false);
            animator.SetBool("EndLoad", true);
        }
    }

    public void PlayEndSceneLoadAnimation()
    {
        if (!animator.GetBool("EndSceneLoad"))
        {
            animator.SetBool("StartLoad", false);
            animator.SetBool("EndLoad", false);
            animator.SetBool("EndSceneLoad", true);
        }
    }

    public void EndLoad()
    {
        loading_bar.EndLoad();
    }
}
