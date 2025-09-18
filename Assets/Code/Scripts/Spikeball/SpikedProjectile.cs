using System.Collections.Generic;
using UnityEngine;

public class SpikedProjectile : MonoBehaviour
{
    [SerializeField] private float lifespan;
    [SerializeField] private LayerMask layers;

    private Rigidbody rigidbody;
    private float time_passed;
    private List<GameObject> hits;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        gameObject.SetActive(false);
        time_passed = lifespan;
        hits = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        time_passed += Time.deltaTime;
        if (time_passed > lifespan)
        {
            if (hits.Count <= 0)
            {
                EventHandler.InvokeFailedHit(gameObject);
            }
            gameObject.SetActive(false);
        }
    }

    public void add_force(Vector3 direction, float force)
    {
        if (!gameObject.activeSelf) 
        {
            gameObject.SetActive(true);
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.AddForce(direction * force, ForceMode.Impulse);
            time_passed = 0;
        } 
    }

    public float get_time_left()
    {
        return lifespan - time_passed;
    }
}
