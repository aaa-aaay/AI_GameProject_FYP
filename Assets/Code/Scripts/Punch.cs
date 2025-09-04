using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Punch : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private float range;
    [SerializeField] private float punch_cooldown;

    private float time_passed;

    private void Start()
    {
        time_passed = punch_cooldown;
    }

    private void Update()
    {
        time_passed += Time.deltaTime;
    }

    public void punch()
    {
        if (time_passed >= punch_cooldown)
        {
            time_passed = 0;

            Collider[] hits = Physics.OverlapSphere(transform.position, range);

            List<Transform> hit_targets = new List<Transform>();

            hit_targets.Add(transform.root);

            Transform temp;

            for (int i = 0; i < hits.Length; i++)
            {
                temp = hits[i].transform.root;

                if (!hit_targets.Contains(temp))
                {
                    EventHandler.InvokeGotHit(transform.root.gameObject, temp.gameObject, damage);
                    hit_targets.Add(temp);
                }
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
