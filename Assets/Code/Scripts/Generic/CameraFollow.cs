using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject target;
    private Vector3 offset;
    private Vector3 velocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offset = transform.position;
        velocity = Vector3.zero;
    }

    public void set_target(GameObject new_target)
    {
        target = new_target;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.SmoothDamp(transform.position, target.transform.position + offset, ref velocity, 0.25f);
        }
    }
}
