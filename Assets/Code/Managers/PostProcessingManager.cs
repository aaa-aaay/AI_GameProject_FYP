using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PostProcessingManager : MonoBehaviour, IGameService
{
    [SerializeField] private Volume volume;
    [SerializeField] private Volume _tagNightVol;
    private void OnEnable()
    {
        ServiceLocator.Instance.AddService(this, false);
        volume.enabled = false;
        _tagNightVol.enabled = false;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        //ServiceLocator.Instance.RemoveService<PostProcessingManager>();
    }

    public void ShowUIEffects(bool show)
    {
        volume.enabled = show;
    }

    public void ShowTagNightEffects(bool show)
    {
        if(_tagNightVol != null)
        _tagNightVol.enabled = show;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ShowTagNightEffects(false);
    }
}
