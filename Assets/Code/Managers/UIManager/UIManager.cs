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



}
