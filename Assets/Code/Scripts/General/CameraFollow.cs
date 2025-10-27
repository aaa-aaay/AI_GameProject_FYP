using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float smooth_time;
    [SerializeField] private Transform target;

    private Vector3 camera_start_position;
    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        camera_start_position = transform.position;

        velocity = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null)
        {
            transform.position = Vector3.SmoothDamp(transform.position, camera_start_position + target.transform.position, ref velocity, smooth_time);
        }
    }

    public void SetTarget(Transform new_target)
    {
        target = new_target;
    }
}
