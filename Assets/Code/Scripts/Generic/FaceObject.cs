using UnityEngine;

public class FaceObject : MonoBehaviour
{
    [SerializeField] private GameObject target;

    // Update is called once per frame
    void Update()
    {
        transform.forward = target.transform.forward;
    }
}
