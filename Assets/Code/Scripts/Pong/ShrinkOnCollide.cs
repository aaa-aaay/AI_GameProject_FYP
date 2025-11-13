using UnityEngine;

public class ShrinkOnCollide : MonoBehaviour
{
    [SerializeField] float scale_size;
    [SerializeField] float scale_time;

    private void OnTriggerExit(Collider other)
    {
        TempScale temp = other.GetComponent<TempScale>();

        if (temp == null)
        {
            temp = other.gameObject.AddComponent<TempScale>();
            temp.SetScaleMultiplier(scale_size);
            temp.SetScaleTime(scale_time);
        }

        Destroy(gameObject);
    }
}
