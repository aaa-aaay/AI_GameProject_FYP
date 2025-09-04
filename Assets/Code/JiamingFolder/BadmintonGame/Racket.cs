using UnityEngine;

public class Racket : MonoBehaviour
{

    private Collider _collider;


    private void Start()
    {
        _collider = GetComponent<Collider>();
    }

    public void ActivateCollider()
    {
        _collider.enabled = true;
    }

    public void DeactivateCollider()
    {
       _collider.enabled = false;
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Shutter"))
        {
            Destroy(other.gameObject);
        }

        //add force to shuttercock here
        //3 different type of hit??
    }
}
