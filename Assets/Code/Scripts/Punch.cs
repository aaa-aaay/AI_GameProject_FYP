using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Punch : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float range;

    public void punch()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        List<Transform> hit_targets = new List<Transform>();

        hit_targets.Add(transform.root);

        Transform temp;

        for (int i = 0; i < hits.Length; i++)
        {
            temp = hits[i].transform.root;

            if (!hit_targets.Contains(temp))
            {
                EventHandler.InvokeGotHit(gameObject, temp.gameObject, damage);
                hit_targets.Add(temp);
            }
        }
    }

    public void punch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            punch();
        }
    }
}
