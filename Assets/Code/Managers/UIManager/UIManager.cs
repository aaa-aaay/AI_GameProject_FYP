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
    [SerializeField] private LevelCompleteManager _levelCompleteManager;
    [SerializeField] private GameObject _levelCompleteCanvas;
    [SerializeField] private GameObject _levelFailedCanvas;

    [Header("UI References")]
    [SerializeField] private CountDownTimer _countDownTimer;

    [Header("Settings References")]
    [SerializeField] private SettingsManager _settingsManager;


    private InputManager _inputManager;
    private MiniGameSO _miniGame;


    private void OnEnable()
    {
        ServiceLocator.Instance.AddService(this, false);
        _levelSelectCanvasGO.SetActive(false);
        _settingsManager.ToggleSettings(false);

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
        _inputManager.onOpenSettings += ToggleSettingsPage;
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


    public void ToggleLevelCompleteUI(bool open,int starCount = 0)
    {
        _levelCompleteManager.ToggleLevelCompleteCanvas(open,starCount);
        if (_inputManager != null)
            _inputManager.toggleInputActivation(!open);

    }

    public void ToggleLevelFailedUI(bool open)
    {
        _levelCompleteManager.ToggleLevelFailedCanvas(open);
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
        ToggleLevelFailedUI(false);
        ToggleLevelCompleteUI(false);
        MySceneManager sManager = ServiceLocator.Instance.GetService<MySceneManager>();
        sManager.restartScene();
    }

    public void SetMiniGameForTutorial(MiniGameSO minigame)
    {
        _miniGame = minigame;
    }
    public MiniGameSO GetMiniGameForTutorial()
    {
        return _miniGame;
    }


    public void StartCountDownTimer()
    {
        _countDownTimer.StartCountDown();
    }

    public void ToggleSettingsPage()
    {
        if (_settingsManager._isSettingsOpen) _settingsManager.ToggleSettings(false);
        else _settingsManager.ToggleSettings(true);

    }



}
