using UnityEngine;
using UnityEngine.InputSystem;

public class Rotate : MonoBehaviour
{
    [SerializeField] protected float rotation_time;

    protected Quaternion start_rotation;
    protected Quaternion end_rotation;

    protected float time_passed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RotateStart();

        // EventHolder.OnRotate += SetRotation;
    }

    private void OnDestroy()
    {
        // EventHolder.OnRotate -= SetRotation;
    }

    protected void RotateStart()
    {
        start_rotation = transform.rotation;
        end_rotation = transform.rotation;

        time_passed = 0;
    }

    // Update is called once per frame
    void Update()
    {
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

    public virtual void RotateBy(Vector3 rotation_value)
    {
        time_passed = 0;
        start_rotation = transform.rotation;
        end_rotation = transform.rotation * Quaternion.Euler(rotation_value);
    }

    public virtual void SetRotation(Vector3 direction)
    {
        time_passed = 0;
        start_rotation = transform.rotation;
        end_rotation = Quaternion.Euler(direction);
    }

    public void SetRotation(GameObject game_object, Vector3 direction)
    {
        if (game_object == gameObject)
        {
            SetRotation(gameObject, direction);
        }
    }

    public virtual void SetRotationTopDown(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            time_passed = 0;
            start_rotation = transform.rotation;
            end_rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y), Vector3.up);
        }
    }

    public void SetRotationTopDown(InputAction.CallbackContext context)
    {
        SetRotationTopDown(context.ReadValue<Vector2>());
    }
}
