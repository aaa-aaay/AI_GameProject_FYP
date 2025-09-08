using System.Collections.Generic;
using UnityEngine;

public class KillBoundaries : MonoBehaviour
{
    [SerializeField] private List<GameObject> boundaries;

    private void OnCollisionEnter(Collision collision)
    {
        if (boundaries.Contains(collision.gameObject))
        {
            EventHandler.InvokeGotKill(Camera.main.gameObject, gameObject);
        }
    }
}
