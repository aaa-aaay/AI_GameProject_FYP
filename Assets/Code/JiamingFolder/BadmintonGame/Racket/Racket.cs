
using System;
using System.Collections.Generic;
using UnityEngine;

public class Racket : MonoBehaviour
{

    private Collider _collider;

    [SerializeField] private BadmintonCourtTargets _targets;
    [SerializeField] private bool _isOpponent;
    [SerializeField] private LastHitChecker _lastHitChecker;


    private int _shotDirection;
    private bool hitShutter;

    public event Action OnHitShutter;
    public event Action OnMissShutter;



    public enum ShotType
    {
        None,
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
        hitShutter = false;
    }

    public void DeactivateCollider()
    {
       _collider.enabled = false;
        if (hitShutter)
        {
            OnHitShutter?.Invoke();
        }
        else
        {
            OnMissShutter?.Invoke();
        }
        hitShutter = false;

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
            hitShutter = true;
            _lastHitChecker.SetLastHitRacket(gameObject);
            DeactivateCollider();
            ServiceLocator.Instance.GetService<AudioManager>().PlaySFX("BMT_Hit",transform.position);

        }

        //add force to shuttercock here
        //3 different type of hit??
    }


    private void ShootWithTechnique(Collider other)
    {

        Shot shot;
        ShotType type;
        List<Transform> FinalShotTarget = new List<Transform>();

        switch (_currentShotType)
        {

            case ShotType.Lob:
                shot = other.GetComponent<LobShot>();
                FinalShotTarget = _isOpponent ? _targets.backTargetsBlue : _targets.backTargetsRed;
                type = ShotType.Lob;
                break;


            case ShotType.Drop:
                shot = other.GetComponent<DropShot>();
                FinalShotTarget = _isOpponent ? _targets.frontTargetsBlue : _targets.frontTargetsRed;
                type = ShotType.Drop;
                break;


            case ShotType.Smash:
                shot = other.GetComponent<SmashShot>();
                FinalShotTarget = _isOpponent ? _targets.middleTargetsBlue : _targets.middleTargetsRed;
                type = ShotType.Smash;
                break;

            default:
                shot = other.GetComponent<LobShot>();
                FinalShotTarget = _isOpponent ? _targets.backTargetsBlue : _targets.backTargetsRed;
                type = ShotType.Lob;
                break;

        }

        //add in the direction set here?
        //1 for left
        //2 for right
        //3 for random or closest???
        shot.ExecuteShot(FinalShotTarget, _shotDirection);

        other.GetComponent<ShotTypeTracker>().setShotType(type);



    }
}
