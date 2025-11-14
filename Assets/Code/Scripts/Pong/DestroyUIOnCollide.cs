using UnityEngine;

public class DestroyUIOnCollide : MonoBehaviour
{
    private void OnCollisionExit(Collision collision)
    {
        PongUI.instance.UpdateSlider(gameObject, 0);
    }
}
