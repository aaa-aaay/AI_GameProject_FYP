using System;
using TMPro;
using UnityEngine;

public class BadmintionGameManager : MonoBehaviour
{
    [SerializeField] private int ScoreToWin = 21;
    private int player1Score = 0;
    private int player2Score = 0;

    [SerializeField] private GameObject shutter;

    [SerializeField] private TMP_Text _P1ScoreDisplay;
    [SerializeField] private TMP_Text _P2ScoreDisplay;


    [SerializeField] private Transform shutterSpawnPoint1;
    [SerializeField] private Transform shutterSpawnPoint2;


    [SerializeField] private BadmintonAgent trainAgent;

    private void Start()
    {
        _P1ScoreDisplay.text = 0.ToString();
        _P2ScoreDisplay.text = 0.ToString();


        shutter.transform.position = shutterSpawnPoint1.position;
    }
    public void PlayerScores(int playerNo)
    {

        if(playerNo == 1)
        {
            player1Score++;
            _P1ScoreDisplay.text = player1Score.ToString();
        }
        else if(playerNo == 2)
        {
            player2Score++;
            _P2ScoreDisplay.text = player2Score.ToString();
        }

        foreach(Shot s in shutter.GetComponents<Shot>())
        {
            s.Cancel();
        }
        shutter.transform.position = playerNo == 1 ? shutterSpawnPoint1.position : shutterSpawnPoint2.position;

        CheckForWin();
    }

    private void CheckForWin()
    {
       if(player1Score >= ScoreToWin)
       {
            ResetGame();
       }
       else if(player2Score >= ScoreToWin)
       {
            ResetGame();
        }
    }

    public void ResetGame()
    {
        player1Score = player2Score = 0;
        _P2ScoreDisplay.text = player2Score.ToString();
        _P1ScoreDisplay.text = player1Score.ToString();

        if(trainAgent != null)
        {
            trainAgent.EndEpisode();
        }
    }
}
