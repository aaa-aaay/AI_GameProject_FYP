using TMPro;
using Unity.VisualScripting;
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
        InputManager inputManager = ServiceLocator.Instance.GetService<InputManager>();
        inputManager.OnJump += OnSpacePressed;
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
        _levelCompleteCanvasOpen = true;
    }

    public void ToggleLevelFailedUI(bool open)
    {
        _levelFailedCanvas.SetActive(open);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ToggleLevelFailedUI(false);
        ToggleLevelCompleteUI(false);
        HideLevelSelectUI();
    }
    private void OnSpacePressed()
    {
        if (_levelCompleteCanvasOpen)
        {
            _levelCompleteCanvasOpen = false;
            MySceneManager sManager = ServiceLocator.Instance.GetService<MySceneManager>();

            sManager.GoBacktoGameLobby();
        }
    }
    public void RestartScene()
    {
        _levelFailedCanvas.SetActive(false);
        MySceneManager sManager = ServiceLocator.Instance.GetService<MySceneManager>();
        sManager.restartScene();
    }



}
