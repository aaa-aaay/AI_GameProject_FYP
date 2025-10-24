using UnityEngine;

public class ResetCarPosition : MonoBehaviour
{

    private Vector3 _originalPosition;
    private Vector3 _originalRoation;
    private Rigidbody _rigidbody;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _originalPosition = transform.localPosition;
        _originalRoation = transform.localEulerAngles;
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void ResetPos()
    {
        transform.localPosition = _originalPosition;
        transform.localEulerAngles = _originalRoation;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }
}
