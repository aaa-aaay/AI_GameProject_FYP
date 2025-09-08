using UnityEngine;

public struct BallData
{
    private GameObject ball;
    private Rigidbody rigidbody;

    public GameObject get_ball()
    {
        return ball;
    }

    public Rigidbody get_rigidbody()
    {
        return rigidbody;
    }

    public void set_rigidbody(Rigidbody new_rigidbody)
    {
        rigidbody = new_rigidbody;
    }

    public BallData(GameObject new_ball, Rigidbody new_rigidbody)
    {
        ball = new_ball;
        rigidbody = new_rigidbody;
    }
}
