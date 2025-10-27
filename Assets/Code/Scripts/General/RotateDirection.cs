using UnityEngine;

public class RotateDirection : Rotate
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RotateStart();
    }

    // Update is called once per frame
    void Update()
    {
        RotateUpdate();
    }

    public override void SetRotation(Vector3 direction)
    {
        time_passed = 0;
        start_rotation = transform.rotation;
        end_rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
