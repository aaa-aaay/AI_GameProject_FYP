using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    [SerializeField] GameObject _RacersGO;
    [SerializeField] GameObject _checkPointHolderGO;
    public int lapsPerRace = 3;
    public int amtofCheckpoints;
    private List<GoalChecker> _racers = new List<GoalChecker>();
    public List<Transform> checkPoints = new List<Transform>();
    private int finishedRacers = 0;

    private void Awake()
    {
        //get all racers' goal checkers
        foreach(Transform child in _RacersGO.transform)
        {
            GoalChecker gc = child.GetComponentInChildren<GoalChecker>();
            if(gc != null)
            {
                _racers.Add(gc);
                gc.OnRaceFinished += handleCarFinsih;
            }
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
        finishedRacers++;

        if (finishedRacers >= _racers.Count)
        {
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
    }


}
