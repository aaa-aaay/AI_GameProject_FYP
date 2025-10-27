using UnityEngine;

public class PositionMovement : Movement
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MovementStart();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * movespeed * Time.deltaTime;
    }
}
