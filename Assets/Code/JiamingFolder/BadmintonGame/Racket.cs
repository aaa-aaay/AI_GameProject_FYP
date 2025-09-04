using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Racket : MonoBehaviour
{

    private Collider _collider;

    [SerializeField]
    private BadmintonCourtTargets _targets;


    public enum ShotType
    {
        Lob,
        Drop,
        Smash
    }

    private ShotType _currentShotType;

    private void Awake()
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


    public void AssignType(ShotType type)
    {
        _currentShotType = type;
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Shutter"))
        {
            ShootWithTechnique(other);
            DeactivateCollider();
        }

        //add force to shuttercock here
        //3 different type of hit??
    }


    private void ShootWithTechnique(Collider other)
    {

        Shot shot;

        switch (_currentShotType)
        {
            case ShotType.Lob:
                shot = other.GetComponent<LobShot>();
                shot.ExecuteShot(_targets.backTargetsRed);
                break;

            case ShotType.Drop:
                shot = other.GetComponent<DropShot>();
                shot.ExecuteShot(_targets.frontTargetsRed);
                break;

            case ShotType.Smash:
                shot = other.GetComponent<SmashShot>();
                shot.ExecuteShot(_targets.backTargetsRed);
                break;

            default:
                shot = other.GetComponent<LobShot>();
                shot.ExecuteShot(_targets.backTargetsRed);
                break;

        }
    }
}
