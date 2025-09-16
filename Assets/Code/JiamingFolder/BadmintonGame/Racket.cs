
using System;
using System.Collections.Generic;
using UnityEngine;

public class Racket : MonoBehaviour
{

    private Collider _collider;

    [SerializeField]
    private BadmintonCourtTargets _targets;

    [SerializeField] private bool _isOpponent;
    [SerializeField] private BadmintionGameManager _gameManager;


    public event Action OnHitShutter;
    private int _shotDirection;


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
    public void AssignShot(ShotType type, int shotDirection)
    {
        _currentShotType = type;
        _shotDirection = shotDirection;
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Shutter"))
        {
            ShootWithTechnique(other);
            OnHitShutter?.Invoke();
            DeactivateCollider();
        }

        //add force to shuttercock here
        //3 different type of hit??
    }


    private void ShootWithTechnique(Collider other)
    {

        Shot shot;
        List<Transform> FinalShotTarget = new List<Transform>();

        switch (_currentShotType)
        {

            case ShotType.Lob:
                shot = other.GetComponent<LobShot>();
                FinalShotTarget = _isOpponent ? _targets.backTargetsBlue : _targets.backTargetsRed;
                break;


            case ShotType.Drop:
                shot = other.GetComponent<DropShot>();
                FinalShotTarget = _isOpponent ? _targets.frontTargetsBlue : _targets.frontTargetsRed;
                break;


            case ShotType.Smash:
                shot = other.GetComponent<SmashShot>();
                FinalShotTarget = _isOpponent ? _targets.backTargetsBlue : _targets.backTargetsRed;
                break;

            default:
                shot = other.GetComponent<LobShot>();
                FinalShotTarget = _isOpponent ? _targets.backTargetsBlue : _targets.backTargetsRed;
                break;

        }

        //add in the direction set here?
        //1 for left
        //2 for right
        //3 for random or closest???
        shot.ExecuteShot(FinalShotTarget, _shotDirection);



    }
}
