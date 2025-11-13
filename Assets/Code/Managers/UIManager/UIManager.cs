using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, IGameService
{
    [Header("Level Select UI")]
    [SerializeField] private GameObject _levelSelectCanvasGO;
    [SerializeField] private TMP_Text _levelNameText;
    [SerializeField] private Image[] starImages;
    [SerializeField] private Sprite _starFilledSprite;
    [SerializeField] private Sprite _starUnFilledSprite;

    [Header("Level Select UI")]
    [SerializeField] private LevelCompleteManager _levelCompleteManager;

    [Header("UI References")]
    [SerializeField] private CountDownTimer _countDownTimer;

    [Header("Settings References")]
    [SerializeField] private SettingsManager _settingsManager;


    private InputManager _inputManager;
    private MiniGameSO _miniGame;
    public event Action<bool> OnUIToFocusToggle;


    private void OnEnable()
    {
        ServiceLocator.Instance.AddService(this, false);
        _levelSelectCanvasGO.SetActive(false);

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

    public void OpenLevelSelectUI(string levelName, int starUnlocked)
    {
        _levelSelectCanvasGO.SetActive(true);
        _levelNameText.text = levelName;


        int count = starUnlocked;
        foreach (Image image in starImages)
        {

            if (count > 0) image.sprite = _starFilledSprite;
            else image.sprite = _starUnFilledSprite;
            count--;
        }
    }

    public void HideLevelSelectUI()
    {
        _levelSelectCanvasGO.SetActive(false);
    }


    public void ToggleLevelCompleteUI(bool open,int starCount = 0)
    {
        //if (open) Time.timeScale = 0;
        //else Time.timeScale = 1;
        OnUIToFocusToggle?.Invoke(open);
        ServiceLocator.Instance.GetService<PostProcessingManager>().ShowUIEffects(open);
        _levelCompleteManager.ToggleLevelCompleteCanvas(open, starCount, _miniGame);
        if (_inputManager != null)
            _inputManager.toggleInputActivation(!open);

    }

    public void ToggleLevelFailedUI(bool open)
    {
        //if (open) Time.timeScale = 0;
        //else Time.timeScale = 1;
        OnUIToFocusToggle?.Invoke(open);
        ServiceLocator.Instance.GetService<PostProcessingManager>().ShowUIEffects(open);
        _levelCompleteManager.ToggleLevelFailedCanvas(open, _miniGame);
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
        bool temp = false;
        if (_settingsManager._isSettingsOpen) temp = false;
        else temp = true;

        _settingsManager.ToggleSettings(temp);
        OnUIToFocusToggle?.Invoke(temp);


    }



}
