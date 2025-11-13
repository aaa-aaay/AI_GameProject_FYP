using System;
using System.Collections.Generic;
using UnityEngine;

public class GoalChecker : MonoBehaviour
{


    [SerializeField] RaceManager _raceManager;


    private int currentLap = 0;
    public int currentCheckPointNo;
    private bool _raceOver = false;
    public event Action OnRaceFinished;
    public event Action onCheckPointHit;

    public Transform currentCheckPoint;

    private void Start()
    {
        ResetCar();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_raceOver) return;

        if (other.CompareTag("RacingGoal"))
        {

            if (currentCheckPointNo < _raceManager.amtofCheckpoints) return; //haven't hit all checkpoints yet

            currentLap++;
            
            if (currentLap >= _raceManager.lapsPerRace)
            {
                _raceOver = true;
                OnRaceFinished?.Invoke();
                Debug.Log("race over");
                //race over for this car
            }
            currentCheckPointNo = 0;


        }

        if (other.CompareTag("RaceCP"))
        {
            if(other.gameObject.GetComponent<RacingGoal>().checkPointNo == currentCheckPointNo)
            {
                currentCheckPointNo++;
                if(currentCheckPointNo < _raceManager.checkPoints.Count)
                {
                    currentCheckPoint = _raceManager.checkPoints[currentCheckPointNo];
                }
                else
                {
                    currentCheckPoint = _raceManager.raceGoalTrans; //finished all checkpoints in lap
                }
                onCheckPointHit?.Invoke();

            }
        }
    }

    public void ResetCar()
    {
        _raceOver = false;
        currentLap = 0;
        currentCheckPointNo = 0;
        currentCheckPoint = _raceManager.checkPoints[currentCheckPointNo];
    }

    public Transform GetCurrentCheckPoint()
    {
        return currentCheckPoint;
    }
}
