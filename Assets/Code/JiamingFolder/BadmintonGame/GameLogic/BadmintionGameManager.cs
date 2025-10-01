using System;
using TMPro;
using UnityEngine;

public class BadmintionGameManager : MonoBehaviour
{
    [SerializeField] private int ScoreToWin = 21;
    public int player1Score = 0;
    public int player2Score = 0;

    [SerializeField] private GameObject shutter;

    [SerializeField] private TMP_Text _P1ScoreDisplay;
    [SerializeField] private TMP_Text _P2ScoreDisplay;


    [SerializeField] private Transform shutterSpawnPoint1;
    [SerializeField] private Transform shutterSpawnPoint2;

    public event Action OnGameOver;

    public event Action OnPlayer1Score;
    public event Action OnPlayer2Score;


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

            OnPlayer1Score?.Invoke();

        }
        else if(playerNo == 2)
        {
            player2Score++;
            _P2ScoreDisplay.text = player2Score.ToString();

            shutter.transform.position = shutterSpawnPoint2.position;
            InRedCourt = true;

            OnPlayer2Score?.Invoke();
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
            GameOver(1);
            //ResetGame();
       }
       else if(player2Score >= ScoreToWin)
       {
            GameOver(2);
            //ResetGame();
        }
    }

    public void ResetGame()
    {
        player1Score = player2Score = 0;
        _P2ScoreDisplay.text = player2Score.ToString();
        _P1ScoreDisplay.text = player1Score.ToString();
        shutter.transform.position = shutterSpawnPoint1.position;
        OnGameOver?.Invoke();
    }

    public void GameOver(int playerNo)
    {
        UIManager uiManager = ServiceLocator.Instance.GetService<UIManager>();
        if (playerNo == 1) {

            uiManager.ToggleLevelCompleteUI(true);
            //player 1 won, go to level Complete UI

        }
        else
        {
            uiManager.ToggleLevelFailedUI(true);
            //AI, go to fail Level UI
        }
    }
}
