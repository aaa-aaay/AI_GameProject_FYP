using System;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    [SerializeField] GameObject _RacersGO;
    [SerializeField] GameObject _checkPointHolderGO;
    public Transform raceGoalTrans;
    [SerializeField] private List<Transform> _startPositions = new List<Transform>();

    public int lapsPerRace = 3;
    public int amtofCheckpoints;
    private List<GoalChecker> _racers = new List<GoalChecker>();
    [HideInInspector] public List<Transform> checkPoints = new List<Transform>();

   
    private int finishedRacers = 0;

    public event Action onRaceOver;

    private void Awake()
    {


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

        }

        foreach(Transform cp in _checkPointHolderGO.transform)
        {
            amtofCheckpoints++;
            checkPoints.Add(cp);
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
        Debug.Log("car finished");
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

        foreach (GoalChecker gc in _racers)
        {
            gc.ResetCar();
        }


        foreach(Transform child in _RacersGO.transform)
        {
            int index = child.GetSiblingIndex();
            if (_startPositions[index] != null)
                child.position = _startPositions[index].position;
        }

    }


}
