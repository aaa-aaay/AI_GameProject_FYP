using System;
using System.Collections.Generic;
using UnityEngine;

public class GoalChecker : MonoBehaviour
{


    [SerializeField] RaceManager _raceManager;


    private int currentLap = 0;
    private int currentCheckPoint;
    private bool _raceOver = false;
    public event Action OnRaceFinished;
    public event Action<Transform> onCheckPointHit;

    private void Start()
    {
        ResetCar();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_raceOver) return;

        if (other.CompareTag("RacingGoal"))
        {

            if (currentCheckPoint < _raceManager.amtofCheckpoints) return; //haven't hit all checkpoints yet

            currentLap++;
            
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
                if(_raceManager.checkPoints[currentCheckPoint] != null)
                {
                    onCheckPointHit?.Invoke(_raceManager.checkPoints[currentCheckPoint]);
                }
                else
                {
                    onCheckPointHit?.Invoke(_raceManager.raceGoalTrans);
                }
                

                //theres a bug now when there is no next checkpoint (ie finished all checkpoints in lap)
            }
            else
            {
                return;
            }
        }
    }

    public void ResetCar()
    {
        _raceOver = false;
        currentLap = 0;
        currentCheckPoint = 0;
        onCheckPointHit?.Invoke(_raceManager.checkPoints[currentCheckPoint]);
    }
}
