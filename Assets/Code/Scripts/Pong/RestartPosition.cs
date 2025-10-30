using UnityEngine;

public class RestartPosition : MonoBehaviour
{
    private Vector3 start_position;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        start_position = transform.position;

        EventHolder.OnRestart += Restart;
    }

    private void OnDestroy()
    {
        EventHolder.OnRestart -= Restart;
    }

    public void Restart()
    {
        transform.position = start_position;
    }

    public void Restart(GameObject player)
    {
        if (player == gameObject)
        {
            Restart();
        }
    }
}
