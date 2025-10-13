using UnityEngine;

public class DestroyAfterCollision : MonoBehaviour
{
    private void OnCollisionExit(Collision collision)
    {
        Destroy(gameObject);
    }
}
