using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform player;   

    [Header("Settings")]
    public Vector3 offset = new Vector3(0f, 5f, -5f); 
    public float rotationX = 45f; 

    void LateUpdate()
    {
        if (player == null) return;

    
        transform.position = player.position + offset;

     
        transform.rotation = Quaternion.Euler(rotationX, 0f, 0f);
    }
}
