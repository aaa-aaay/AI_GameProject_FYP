using System;
using TMPro;
using UnityEngine;

public class BadmintionGameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private int ScoreToWin = 21;
    public int player1Score = 0;
    public int player2Score = 0;

    [Header("references")]
    [SerializeField] private GameObject shutter;
    [SerializeField] private GameObject _serveUI;

    [SerializeField] private TMP_Text _P1ScoreDisplay;
    [SerializeField] private TMP_Text _P2ScoreDisplay;

    [SerializeField] private Transform player1;
    [SerializeField] private Transform player2;

    [SerializeField] private Transform shutterSpawnPoint1;
    [SerializeField] private Transform shutterSpawnPoint2;
    [SerializeField] private Transform ServeLocation1;
    [SerializeField] private Transform ServeLocation2;
    public event Action OnGameOver;

    public event Action OnPlayer1Score;
    public event Action OnPlayer2Score;

    public bool serving;
    //public bool InRedCourt { get; set; } = true;


    private void Start()
    {
        _P1ScoreDisplay.text = 0.ToString();
        _P2ScoreDisplay.text = 0.ToString();
        StartServe(1);

        foreach(Racket rkt in GetComponentsInChildren<Racket>())
        {
            rkt.OnHitShutter += ShutterHit;
        }

    }


    public void PlayerScores(int playerNo)
    {

        if(playerNo == 1)
        {
            player1Score++;
            _P1ScoreDisplay.text = player1Score.ToString();
            StartServe(1);
            OnPlayer1Score?.Invoke();

        }
        else if(playerNo == 2)
        {
            player2Score++;
            _P2ScoreDisplay.text = player2Score.ToString();
            StartServe(2);
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
        if (player1Score >= ScoreToWin)
        {
            ResetGame();
            GameOver(1);
            
        }
       else if(player2Score >= ScoreToWin)
       {
            ResetGame();
            GameOver(2);
            
       }
    }

    public void ResetGame()
    {
        player1Score = player2Score = 0;
        _P2ScoreDisplay.text = player2Score.ToString();
        _P1ScoreDisplay.text = player1Score.ToString();
        OnGameOver?.Invoke();
        StartServe(1);
    }

    public void GameOver(int playerNo)
    {
        MiniGameOverHandler handler = GetComponent<MiniGameOverHandler>();

        if (playerNo == 1) {


            handler.HandleGameOver(false);
        }
        else
        {
            handler.HandleGameOver(true, 2, 3);
            
        }
    }


    private void StartServe(int pNo)
    {
        serving = true;
        player1.transform.position = ServeLocation1.position;
        player2.transform.position = ServeLocation2.position;

        Vector3 uiPos;
        if (pNo == 1)
        {
            shutter.transform.position = shutterSpawnPoint1.position;
            uiPos = ServeLocation1.position;

        }
        else
        {
            shutter.transform.position = shutterSpawnPoint2.position;
            uiPos = ServeLocation2.position;
        }
        _serveUI.SetActive(true);
        _serveUI.transform.position = uiPos + new Vector3(0,2.5f,0);

    }

    private void ShutterHit()
    {
        if(!serving) return;
        _serveUI.SetActive(false);
        serving = false;
    }
}
