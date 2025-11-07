using UnityEngine;

public class Fall : MonoBehaviour
{
    [SerializeField] private float movespeed;

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = Vector3.down * Time.deltaTime * movespeed;
        transform.position += movement;
    }
}
