using UnityEngine;

public class MiniGameOverHandler : MonoBehaviour, IGameService
{
    private UIManager uiManager;
    private SaveLoadManager slManager;
    [SerializeField] private bool _restart;


    private void Start()
    {
        uiManager = ServiceLocator.Instance.GetService<UIManager>();
        slManager = ServiceLocator.Instance.GetService<SaveLoadManager>();
    }

    public void HandleGameOver(bool isWin, int levelNo = 0, int startCount = 0)
    {
        if (_restart) return;

        if (isWin)
        {
            uiManager.ToggleLevelCompleteUI(true);
            slManager.SaveData(levelNo, startCount);
        }
        else
        {
            uiManager.ToggleLevelFailedUI(true);
        }
    }



}
