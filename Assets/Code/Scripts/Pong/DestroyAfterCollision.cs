using System.Collections;
using UnityEngine;

public class DestroyAfterCollision : MonoBehaviour
{
    private void OnCollisionExit(Collision collision)
    {
        StartCoroutine(DelayDestroy());
    }

    IEnumerator DelayDestroy()
    {
        yield return null;
        Destroy(gameObject);
    }
}
