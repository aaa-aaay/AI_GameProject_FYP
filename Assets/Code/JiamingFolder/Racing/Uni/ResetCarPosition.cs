using UnityEngine;

public class ResetCarPosition : MonoBehaviour
{

    [SerializeField] private RaceManager manager;

    private Vector3 _originalPosition;
    private Rigidbody _rigidbody;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager.onRaceOver += ResetPos;
        _originalPosition = transform.localPosition;
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void ResetPos()
    {
        transform.localPosition = _originalPosition;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }
}
