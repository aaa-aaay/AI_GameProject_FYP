using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingManager : MonoBehaviour, IGameService
{
    [SerializeField] private Volume volume;
    private void OnEnable()
    {
        ServiceLocator.Instance.AddService(this, false);
        volume.enabled = false;
    }

    private void OnDestroy()
    {
        //ServiceLocator.Instance.RemoveService<PostProcessingManager>();
    }

    public void ShowUIEffects(bool show)
    {
        volume.enabled = show;
    }
}
