using UnityEngine;

public class SpawnAfterCollision : MonoBehaviour
{
    [SerializeField] private GameObject wall;

    private void OnTriggerExit(Collider other)
    {
        Instantiate(wall).transform.position = transform.position;
        Destroy(gameObject);
    }
}
