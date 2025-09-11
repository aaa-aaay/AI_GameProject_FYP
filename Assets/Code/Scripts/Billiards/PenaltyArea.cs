using UnityEngine;

public class PenaltyArea : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        EventHandler.InvokeOutOfBounds(collision.gameObject);
    }
}
