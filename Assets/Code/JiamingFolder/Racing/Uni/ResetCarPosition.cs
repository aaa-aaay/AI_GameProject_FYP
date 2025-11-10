using UnityEngine;

public class ResetCarPosition : MonoBehaviour
{
    [SerializeField] private GameObject _carMesh;

    private Vector3 _originalPositionMesh;
    private Quaternion _originalRoationMesh;

    private Vector3 _originalPosition;
    private Quaternion _originalRoation;
    private Rigidbody _rigidbody;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _originalPosition = transform.localPosition;
        _originalRoation = transform.localRotation;

        _originalPositionMesh = _carMesh.transform.localPosition;
        _originalRoationMesh = _carMesh.transform.localRotation;
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void ResetPos()
    {
        transform.localPosition = _originalPosition;
        transform.localRotation = _originalRoation;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        _carMesh.transform.localPosition = _originalPositionMesh;
        _carMesh.transform.localRotation = _originalRoationMesh;
    }
}
