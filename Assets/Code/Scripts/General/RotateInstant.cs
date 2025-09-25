using UnityEngine;

public class RotateInstant : Rotate
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RotateStart();
    }

    private void Update()
    {
        
    }

    public override void RotateBy(Vector3 rotation_value)
    {
        transform.rotation = transform.rotation * Quaternion.Euler(rotation_value);
    }

    public override void SetRotation(Vector3 direction)
    {
        transform.rotation = Quaternion.Euler(direction);
    }
}
