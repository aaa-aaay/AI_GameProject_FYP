using System;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    [SerializeField] GameObject _RacersGO;
    [SerializeField] GameObject _checkPointHolderGO;
    [SerializeField] GameObject _startPosHolderGO;
    public Transform raceGoalTrans;
    private List<Transform> _startPositions = new List<Transform>();

    [Header("Race Settings")]
    public bool isDebugMood;
    public int lapsPerRace = 3;
    [HideInInspector] public int amtofCheckpoints;
    private List<GoalChecker> _racers = new List<GoalChecker>();
    [HideInInspector]public List<Transform> checkPoints = new List<Transform>();

   
    private int finishedRacers = 0;

    public event Action onRaceOver;

    private void Awake()
    {

        foreach (Transform sp in _startPosHolderGO.transform)
        {
            _startPositions.Add(sp);
        }

        //get all racers' goal checkers
        foreach (Transform child in _RacersGO.transform)
        {
            GoalChecker gc = child.GetComponentInChildren<GoalChecker>();
            if(gc != null)
            {
                _racers.Add(gc);
                gc.OnRaceFinished += handleCarFinsih;
            }

            //set start positions
            if (_startPositions[_racers.Count - 1] != null)
            child.position = _startPositions[_racers.Count - 1].position;


            //playing SFX 
            ServiceLocator.Instance.GetService<AudioManager>().PlaySFX("TestSound", transform.position);
            //playing BGM
            ServiceLocator.Instance.GetService<AudioManager>().PlayBackgroundMusic("TestSound");


        }

        foreach(Transform cp in _checkPointHolderGO.transform)
        {

            checkPoints.Add(cp);
            RacingGoal goal = cp.GetComponent<RacingGoal>();
            goal.checkPointNo = amtofCheckpoints;
            amtofCheckpoints++;
        }



        finishedRacers = 0;



    }

    private void OnDestroy()
    {
        foreach(GoalChecker gc in _racers)
        {
            gc.OnRaceFinished -= handleCarFinsih;
        }
        _racers.Clear();
    }

    private void handleCarFinsih()
    {

        if (!isDebugMood) return;

        finishedRacers++;

        if (finishedRacers >= _racers.Count)
        {

            onRaceOver?.Invoke();
            //restart or end    
            Restart();

        }
    }

    private void Restart()
    {
        finishedRacers = 0;


        //Reset all cars in thier own scripts


        //foreach (GoalChecker gc in _racers)
        //{
        //    gc.ResetCar();
        //}




        //foreach(Transform child in _RacersGO.transform)
        //{
        //    int index = child.GetSiblingIndex();
        //    if (_startPositions[index] != null)
        //        child.position = _startPositions[index].position;
        //}

    }



}
