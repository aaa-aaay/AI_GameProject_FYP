using UnityEngine;

public class RotateMovement : Movement
{
    [SerializeField] private float rotation_time;

    private Quaternion start_rotation;
    private Quaternion end_rotation;

    private float time_passed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MovementStart();

        start_rotation = transform.rotation;
        end_rotation = transform.rotation;

        time_passed = 0;

        // EventHolder.OnMove += SetDirection;
    }

    private void OnDestroy()
    {
        // EventHolder.OnMove -= SetDirection;
    }

    // Update is called once per frame
    void Update()
    {
        MoveUpdate();
        RotateUpdate();
    }

    protected void RotateUpdate()
    {
        time_passed += Time.deltaTime;
        if (time_passed > rotation_time)
        {
            time_passed = rotation_time;
        }
        transform.rotation = Quaternion.Slerp(start_rotation, end_rotation, time_passed / rotation_time);
    }

    protected void SetRotationByDirection()
    {
        time_passed = 0;
        start_rotation = transform.rotation;
        end_rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    public override void SetDirection(Vector3 new_direction)
    {
        base.SetDirection(new_direction);

        SetRotationByDirection();
    }

    public override void AddDirection(Vector3 added_direction)
    {
        base.AddDirection(added_direction);

        SetRotationByDirection();
    }
}
