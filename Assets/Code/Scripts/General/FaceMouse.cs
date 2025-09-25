using UnityEngine;

public class FaceMouse : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 direction = hit.point;
            direction.y = transform.position.y;

            transform.LookAt(direction, Vector3.up);
        }
    }
}
