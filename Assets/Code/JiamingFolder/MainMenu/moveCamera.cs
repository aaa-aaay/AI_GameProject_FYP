using UnityEngine;

public class moveCamera : MonoBehaviour
{
    [SerializeField] float _moveSpeed;


    private void Start()
    {
        
    }

    private void Update()
    {
        transform.position += Vector3.forward * _moveSpeed * Time.deltaTime;
    }
}
