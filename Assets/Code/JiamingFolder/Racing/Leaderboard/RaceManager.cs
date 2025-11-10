using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaceManager : MonoBehaviour
{
    [SerializeField] GameObject _RacersGO;
    [SerializeField] GameObject _checkPointHolderGO;
    [SerializeField] GameObject _startPosHolderGO;
    [SerializeField] private RacingLeaderboard _leaderboard;

    public Transform raceGoalTrans;
    private List<Transform> _startPositions = new List<Transform>();


    [Header("Ranking placement references")]
    [SerializeField] private List<Sprite> _placementSprites = new List<Sprite>();
    [SerializeField] private Image _placementImage;

    [Header("Race Settings")]
    public bool isDebugMood;
    private float _finishTimeDebug;
    public int lapsPerRace = 3;
    [HideInInspector] public int amtofCheckpoints;
    private List<GoalChecker> _racers = new List<GoalChecker>();
    [HideInInspector]public List<Transform> checkPoints = new List<Transform>();

   
    private int finishedRacers = 0;

    public event Action OnResetRace;

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
                gc.OnRaceFinished += HandleCarfinishRace;
            }

            //set start positions
            if (_startPositions[_racers.Count - 1] != null)
            child.position = _startPositions[_racers.Count - 1].position;


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

    private void Start()
    {
        ServiceLocator.Instance.GetService<UIManager>().StartCountDownTimer();
        _finishTimeDebug = 1000;
    }

    private void OnDestroy()
    {
        foreach(GoalChecker gc in _racers)
        {
            gc.OnRaceFinished -= HandleCarfinishRace;
        }
        _racers.Clear();
    }


    private void Update()
    {
        _racers = _racers.OrderByDescending(r => r.currentLap)
            .ThenByDescending(r => r.currentCheckPointNo)
            .ThenBy(r => Vector3.Distance(r.transform.position, r.currentCheckPoint.position)).ToList();

        foreach(GoalChecker racers in _racers)
        {
            if (racers.GetRacerName().Contains("you")){
                _placementImage.sprite = _placementSprites[_racers.IndexOf(racers)];
            }
        }
    }
    private void HandleCarfinishRace(string name, float timeTaken)
    {

        if (timeTaken < _finishTimeDebug)
        {
            _finishTimeDebug = timeTaken;
            Debug.Log(_finishTimeDebug);
        }

        if (isDebugMood) return;

        finishedRacers++;
        _leaderboard.AddLeaderboardData(name, timeTaken);
        Debug.Log("racers finished");
        if (finishedRacers >= _racers.Count)
        {
            //restart or end
            Restart();

        }
    }

    private void Restart()
    {
        OnResetRace?.Invoke();
        finishedRacers = 0;

    }

    public void OpenLeaderBoard()
    {
        if(!isDebugMood) _leaderboard.ShowLeaderBoard();
    }

}
