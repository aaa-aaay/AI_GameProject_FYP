using Unity.VisualScripting;
using UnityEngine;

public class DamageableTrigger : MonoBehaviour
{

    private Rigidbody rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        EventHandler.GotHit(other.gameObject, gameObject, 1);
    }
}
