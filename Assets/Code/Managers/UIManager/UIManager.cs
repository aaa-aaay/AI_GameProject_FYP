using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour, IGameService
{

    public static DialogueManager Instance { get; private set; }

    [Header("Level Select UI")]
    [SerializeField] private GameObject levelSelectCanvasGO;
    [SerializeField] private TMP_Text levelNameText;
    private int starCount;

    private void OnEnable()
    {
        ServiceLocator.Instance.AddService(this, false);
        levelSelectCanvasGO.SetActive(false);
    }

    private void OnDisable()
    {
        ServiceLocator.Instance.RemoveService<UIManager>(false);
    }

    public void OpenLevelSelectUI(string levelName, int StarUnlocked)
    {
        levelSelectCanvasGO.SetActive(true);
        levelNameText.text = levelName;
    }

    public void HideLevelSelectUI()
    {
        levelSelectCanvasGO.SetActive(false);
    }

}
