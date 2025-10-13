using UnityEngine;

public class PreviousPosition : MonoBehaviour
{
    private Vector3 prev_position;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        prev_position = transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        prev_position = transform.position;
    }

    public Vector3 GetDirection()
    {
        return transform.position - prev_position;
    }

    public Vector3 GetPrevPosition()
    {
        return prev_position;
    }
}
