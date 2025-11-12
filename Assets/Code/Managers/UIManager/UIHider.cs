using UnityEngine;

public class UIHider : MonoBehaviour
{
    UIManager _uiManager;
    [SerializeField] Canvas[] canvases;
    private void Start()
    {
        _uiManager = ServiceLocator.Instance.GetService<UIManager>();
        _uiManager.OnUIToFocusToggle += HideUI;
    }
    private void HideUI(bool open)
    {
        foreach (Canvas c in canvases)
            c.enabled = !open;
    }
    private void OnDestroy()
    {
        _uiManager.OnUIToFocusToggle -= HideUI;
    }


}
