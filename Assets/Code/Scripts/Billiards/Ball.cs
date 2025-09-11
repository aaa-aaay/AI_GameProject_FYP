using UnityEngine;

public class Ball : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BilliardSIngleton.instance.set_new_ball(gameObject, GetComponent<Rigidbody>());   
    }
}
