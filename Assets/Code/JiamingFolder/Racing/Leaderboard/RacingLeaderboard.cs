using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RacingLeaderboard : MonoBehaviour
{
    [SerializeField] private GameObject leaderboardUI;
    [SerializeField] private GameObject LeaderboardPanelEntries;
    [SerializeField] private Color _playerPanelColor;
    private List<LeaderboardEntry> _entries;


    private int displayCounter = 0;

    private void Start()
    {
        _entries = new List<LeaderboardEntry>();
        displayCounter = 0;
        foreach (Transform child in LeaderboardPanelEntries.transform)
        {
            LeaderboardEntry entry = child.GetComponent<LeaderboardEntry>();
            if(entry != null)
            {
                _entries.Add(entry);
            }
        }
    }
    public void AddLeaderboardData(string racerName, float raceTime)
    {

        if (displayCounter > _entries.Count) return;
        _entries[displayCounter].SetEntryData(racerName, raceTime);

        if (racerName.Contains("you"))
        {
            _entries[displayCounter].gameObject.GetComponent<Image>().color = _playerPanelColor;

        }

        displayCounter++;
    }

    public void ShowLeaderBoard()
    {
        //sort leaderboard based on race time
        leaderboardUI.SetActive(true);
        
    }
}
