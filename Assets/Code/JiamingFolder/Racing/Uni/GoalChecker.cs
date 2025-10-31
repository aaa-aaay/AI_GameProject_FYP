using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalChecker : MonoBehaviour
{


    [SerializeField] RaceManager _raceManager;
    [SerializeField] private string  _name = "hello";

    private Timer _timer;
    private int currentLap = 0;
    public int currentCheckPointNo;
    private bool _raceOver = false;
    public event Action<string, float> OnRaceFinished;
    public event Action onCheckPointHit;

    public Transform currentCheckPoint;

    private void Start()
    {
        _timer = GetComponent<Timer>();
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
                Debug.Log(_timer.StopTimer());
                OnRaceFinished?.Invoke(_name, _timer.StopTimer());

                if (!_raceManager.isDebugMood)
                {
                    HideCarAfterAwhile();
                }
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
        _timer.StartTimer();
    }

    public Transform GetCurrentCheckPoint()
    {
        return currentCheckPoint;
    }

    IEnumerator HideCarAfterAwhile()
    {
        yield return new WaitForSeconds(2f);
        transform.parent.gameObject.SetActive(false);
    }
}
