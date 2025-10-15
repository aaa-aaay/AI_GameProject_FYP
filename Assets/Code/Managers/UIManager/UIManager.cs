using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour, IGameService
{

    public static DialogueManager Instance { get; private set; }

    [Header("Level Select UI")]
    [SerializeField] private GameObject _levelSelectCanvasGO;
    [SerializeField] private TMP_Text _levelNameText;
    private int starCount;

    [Header("Level Select UI")]
    [SerializeField] private GameObject _levelCompleteCanvas;
    [SerializeField] private GameObject _levelFailedCanvas;
    private bool _levelCompleteCanvasOpen;

    private InputManager _inputManager;


    private void OnEnable()
    {
        ServiceLocator.Instance.AddService(this, false);
        _levelSelectCanvasGO.SetActive(false);
        _levelCompleteCanvasOpen = false;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
       // ServiceLocator.Instance.RemoveService<UIManager>(false);

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
    }

    public void OpenLevelSelectUI(string levelName, int StarUnlocked)
    {
        _levelSelectCanvasGO.SetActive(true);
        _levelNameText.text = levelName;
    }

    public void HideLevelSelectUI()
    {
        _levelSelectCanvasGO.SetActive(false);
    }


    public void ToggleLevelCompleteUI(bool open)
    {
        _levelCompleteCanvas.SetActive(open);
        _levelCompleteCanvasOpen = open;
        if (_inputManager != null)
            _inputManager.toggleInputActivation(!open);

    }

    public void ToggleLevelFailedUI(bool open)
    {
        _levelFailedCanvas.SetActive(open);
        if(_inputManager != null)
            _inputManager.toggleInputActivation(!open);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ToggleLevelFailedUI(false);
        ToggleLevelCompleteUI(false);
        HideLevelSelectUI();
    }

    public void RestartScene()
    {
        _levelFailedCanvas.SetActive(false);
        MySceneManager sManager = ServiceLocator.Instance.GetService<MySceneManager>();
        sManager.restartScene();
    }



}
