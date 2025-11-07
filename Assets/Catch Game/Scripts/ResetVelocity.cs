using UnityEngine;

public class ResetVelocity : MonoBehaviour
{
    Rigidbody body;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = GetComponent<Rigidbody>();

        EventHolder.OnRestart += Restart;
    }

    private void OnDestroy()
    {
        EventHolder.OnRestart -= Restart;
    }

    public void Restart()
    {
        body.linearVelocity = Vector3.zero;
    }

    public void Restart(GameObject player)
    {
        if (player == gameObject)
        {
            Restart(gameObject);
        }
    }
}
