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

    public event Action OnGameOver;

    public bool InRedCourt { get; set; } = true;


    private void Start()
    {
        _P1ScoreDisplay.text = 0.ToString();
        _P2ScoreDisplay.text = 0.ToString();

        shutter.transform.position = shutterSpawnPoint1.position;
        InRedCourt = true;
    }
    public void PlayerScores(int playerNo)
    {

        if(playerNo == 1)
        {
            player1Score++;
            _P1ScoreDisplay.text = player1Score.ToString();

            shutter.transform.position = shutterSpawnPoint1.position;
            InRedCourt = false;

            Debug.Log("Set to red Court");
        }
        else if(playerNo == 2)
        {
            player2Score++;
            _P2ScoreDisplay.text = player2Score.ToString();

            shutter.transform.position = shutterSpawnPoint2.position;
            InRedCourt = true;

            Debug.Log("Set to red Court");
        }

        foreach(Shot s in shutter.GetComponents<Shot>())
        {
            s.Cancel();
        }

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
        OnGameOver?.Invoke();
    }
}
