using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class AirHockeyFSM : MonoBehaviour
{
    [SerializeField] private AirHockeyInstance instance;

    [SerializeField] private UnityEvent<Vector2> Move;

    [SerializeField] private Bound bound;

    [SerializeField] private float z_offset;

    private List<Rigidbody> pucks;

    // Start is called once before the first ezecution of Update after the MonoBehaviour is created
    void Start()
    {
        pucks = instance.GetPuck();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = Vector2.zero;
        Rigidbody closest = GetClosest();

        if (bound.WithinBounds(closest.transform))
        {
            direction.x = closest.transform.localPosition.x - transform.localPosition.x;
            direction.y = closest.transform.localPosition.z - transform.localPosition.z + z_offset;

            Move?.Invoke(direction);
        }
        bound.BindPosition(transform);
    }

    protected Rigidbody GetClosest()
    {
        Rigidbody closest = null;
        for (int i = 0; i < pucks.Count; i++)
        {
            if (closest == null)
            {
                closest = pucks[i];
            }
            else if (Vector3.Dot(pucks[i].linearVelocity, transform.position - pucks[i].transform.position) > 0.5f)
            {
                if (ZDistance(closest.transform) > ZDistance(pucks[i].transform))
                {
                    closest = pucks[i];
                }
            }
        }
        return closest;
    }

    protected float ZDistance(Transform other)
    {
        float distance = transform.position.z - other.position.z;
        if (distance < 0)
        {
            distance *= -1;
        }
        return distance;
    }
}
