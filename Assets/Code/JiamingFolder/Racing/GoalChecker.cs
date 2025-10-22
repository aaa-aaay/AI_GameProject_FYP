using System;
using System.Collections.Generic;
using UnityEngine;

public class GoalChecker : MonoBehaviour
{


    [SerializeField] RaceManager _raceManager;
    [SerializeField] GameObject _lastCheckPoint;


    private int currentLap = 0;
    private int currentCheckPoint;
    private bool _raceOver = false;
    public event Action OnRaceFinished;
    public event Action<Transform> onCheckPointHit;

    private void Start()
    {
        ResetCar();
        onCheckPointHit?.Invoke(_raceManager.checkPoints[currentCheckPoint]);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_raceOver) return;

        if (other.CompareTag("RacingGoal"))
        {

            if (currentCheckPoint < _raceManager.amtofCheckpoints) return; //haven't hit all checkpoints yet

            currentLap++;
            Debug.Log("Goal Reached");
            
            if (currentLap >= _raceManager.lapsPerRace)
            {
                _raceOver = true;
                OnRaceFinished?.Invoke();
                //race over for this car
            }


            currentCheckPoint = 0;


        }

        if (other.CompareTag("RaceCP"))
        {
            if(other.gameObject.GetComponent<RacingGoal>().checkPointNo == currentCheckPoint)
            {
                currentCheckPoint++;
                onCheckPointHit?.Invoke(_raceManager.checkPoints[currentCheckPoint]);
            }
            else{
                return;
            }
        }
    }

    public void ResetCar()
    {
        _raceOver = false;
        currentLap = 0;
        currentCheckPoint = 0;
    }
}
