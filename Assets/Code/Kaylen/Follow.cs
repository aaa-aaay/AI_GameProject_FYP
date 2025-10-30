using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform player;   // drag your Player object here

    [Header("Settings")]
    public Vector3 offset = new Vector3(0f, 5f, -5f); // default offset
    public float rotationX = 45f; // fixed tilt

    void LateUpdate()
    {
        if (player == null) return;

        // follow player with offset
        transform.position = player.position + offset;

        // fixed angle (top-down tilt)
        transform.rotation = Quaternion.Euler(rotationX, 0f, 0f);
    }
}
