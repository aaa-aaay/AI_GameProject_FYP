using UnityEngine;
using System.Collections.Generic;

public class RigidbodySlow : MonoBehaviour
{
    private List<Rigidbody> data = new List<Rigidbody>();

    private void OnDestroy()
    {
        for (int i = 0; i < data.Count; i++)
        {
            data[i].linearVelocity *= 2;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rigid_body = other.gameObject.GetComponent<Rigidbody>();

        if (rigid_body != null)
        {
            if (!data.Contains(rigid_body))
            {
                data.Add(rigid_body);
                rigid_body.linearVelocity *= 0.5f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rigid_body = other.gameObject.GetComponent<Rigidbody>();

        if (rigid_body != null)
        {
            if (data.Contains(rigid_body))
            {
                data.Remove(rigid_body);
                rigid_body.linearVelocity *= 2;
            }
        }
    }
}
