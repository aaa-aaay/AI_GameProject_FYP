using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingManager : MonoBehaviour,IGameService
{
    private Volume volume;
    private void OnEnable()
    {
        ServiceLocator.Instance.AddService(this,false);
    }

    private void OnDestroy() {
        //ServiceLocator.Instance.RemoveService<PostProcessingManager>();
    }





}
