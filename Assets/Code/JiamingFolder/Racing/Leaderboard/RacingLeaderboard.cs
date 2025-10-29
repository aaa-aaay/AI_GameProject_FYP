using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RacingLeaderboard : MonoBehaviour
{

    private List<RaceData> leaderboard;
    [SerializeField] private GameObject leaderboardUI;

    private void Start()
    {
        leaderboard = new List<RaceData>();
    }
    public void AddLeaderboardData(string racerName, float raceTime)
    {
        RaceData tempData = new RaceData();
        tempData.racerName = racerName;
        tempData.raceTime = raceTime;


        leaderboard.Add(tempData);
        // Implementation for adding data to the racing leaderboard
    }

    public void ShowLeaderBoard()
    {

        //sort leaderboard based on race time

        leaderboardUI.SetActive(true);
        
    }



}

public class RaceData
{
    public string racerName;
    public float raceTime;
}
