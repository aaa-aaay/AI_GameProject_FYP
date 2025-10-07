using UnityEngine;

public class FaceDirection : MonoBehaviour
{
    [SerializeField] private float rotation_time = 0.1f;

    private float time_passed;
    private Quaternion start_rotation;
    private Quaternion end_rotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        time_passed = 0;
        start_rotation = transform.rotation;
        end_rotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        time_passed += Time.deltaTime;
        if (time_passed > rotation_time)
        {
            time_passed = rotation_time;
        }
        transform.rotation = Quaternion.Slerp(start_rotation, end_rotation, time_passed / rotation_time);
    }

    public void set_target(Vector3 target_rotation)
    {
        time_passed = 0;
        start_rotation = transform.rotation;
        end_rotation = Quaternion.LookRotation(target_rotation.normalized, Vector3.up);
    }
}
