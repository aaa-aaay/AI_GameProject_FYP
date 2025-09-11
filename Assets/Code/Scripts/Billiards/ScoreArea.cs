using UnityEngine;

public class ScoreArea : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        EventHandler.InvokeScored(collision.gameObject);
    }
}
