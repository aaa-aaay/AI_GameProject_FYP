using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Racket : MonoBehaviour
{

    private Collider _collider;

    [SerializeField]
    private BadmintonCourtTargets _targets;


    private void Start()
    {
        _collider = GetComponent<Collider>();
    }

    public void ActivateCollider()
    {
        _collider.enabled = true;
    }

    public void DeactivateCollider()
    {
       _collider.enabled = false;
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Shutter"))
        {

            Shot shot = other.GetComponent<Shot>();
            shot.ExecuteShot(_targets.backTargetsRed);

            DeactivateCollider();
        }

        //add force to shuttercock here
        //3 different type of hit??
    }
}
