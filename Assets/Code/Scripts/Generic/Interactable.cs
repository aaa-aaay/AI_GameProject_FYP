using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] protected float range = 3f;

    protected GameObject player;
    protected bool inRange;

    protected SphereCollider collide;

    protected void OnEnable()
    {
        player = GameObject.FindWithTag("Player");

        collide = GetComponent<SphereCollider>();
        if (!collide)
            collide = gameObject.AddComponent<SphereCollider>();
        collide.isTrigger = true;
        collide.radius = range;
    }

    protected void OnDisable()
    {
        inRange = false;
    }
                
    protected void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        inRange = true;
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player") return;

        inRange = false;
    }
}
