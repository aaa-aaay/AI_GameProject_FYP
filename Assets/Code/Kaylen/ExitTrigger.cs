using UnityEngine;
using System.Collections;

public class ExitTrigger : MonoBehaviour
{
    [Header("Door Rotation Settings")]
    public Transform doorObject;
    public float openAngle = 120f;
    public float rotationSpeed = 60f;
    public Vector3 rotationAxis = Vector3.up;

    private bool isUnlocked = false;
    private bool isOpen = false;
    private bool isRotating = false;

    public void UnlockDoor()
    {
        if (!isUnlocked)
        {
            isUnlocked = true;
            Debug.Log("Door unlocked!");

            if (!isOpen && !isRotating)
                StartCoroutine(OpenDoor());
        }
    }

    private IEnumerator OpenDoor()
    {
        isRotating = true;
        float startAngle = doorObject.localEulerAngles.y;
        float endAngle = openAngle;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * (rotationSpeed / Mathf.Abs(endAngle - startAngle));
            float angle = Mathf.LerpAngle(startAngle, endAngle, t);
            doorObject.localRotation = Quaternion.Euler(rotationAxis * angle);
            yield return null;
        }

        doorObject.localRotation = Quaternion.Euler(rotationAxis * endAngle);
        isRotating = false;
        isOpen = true;

        Debug.Log("Door opened.");
    }
}
