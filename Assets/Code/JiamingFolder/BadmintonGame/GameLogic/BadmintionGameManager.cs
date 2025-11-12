using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BadmintionGameManager : MonoBehaviour
{
    [SerializeField] private int ScoreToWin = 21;
    [SerializeField] private int _whoServesFirst = 1;
    public int player1Score = 0;
    public int player2Score = 0;

    [SerializeField] private GameObject shutter;

    [SerializeField] private TMP_Text _P1ScoreDisplay;
    [SerializeField] private TMP_Text _P2ScoreDisplay;


    [SerializeField] private Transform shutterSpawnPoint1;
    [SerializeField] private Transform shutterSpawnPoint2;


    [Header("Serve refs")]
    [SerializeField] private GameObject serveUIGO;
    [SerializeField] private GameObject _serveMarkerGO;
    [SerializeField] private List<Transform> serverHolders;
    [SerializeField] private List<Transform> serverMarkerHolders;
    [SerializeField] private List<Racket> _rackets;
    public event Action OnGameOver;

    public event Action OnPlayer1Score;
    public event Action OnPlayer2Score;


    public bool InRedCourt { get; set; } = true;

    private AudioManager audioManager;


    private void Start()
    {
        audioManager = ServiceLocator.Instance.GetService<AudioManager>();
        _P1ScoreDisplay.text = 0.ToString();
        _P2ScoreDisplay.text = 0.ToString();


        ToggleServe(_whoServesFirst);
        InRedCourt = true;

        foreach (Racket r in _rackets)
        {
            r.OnHitShutter += ShutterHit;
        }

    }
    private void OnDestroy()
    {
        foreach (Racket r in _rackets)
        {
            r.OnHitShutter -= ShutterHit;
        }
    }


    public void PlayerScores(int playerNo)
    {
        audioManager.PlaySFX("BMT_Score",Camera.main.transform.position);
        if (playerNo == 1)
        {
            player1Score++;
            _P1ScoreDisplay.text = player1Score.ToString();

            ToggleServe(1);
            InRedCourt = false;

            OnPlayer1Score?.Invoke();

        }
        else if(playerNo == 2)
        {
            player2Score++;
            _P2ScoreDisplay.text = player2Score.ToString();


            ToggleServe(2);
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
        ToggleServe(_whoServesFirst);
        OnGameOver?.Invoke();
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

    private void ToggleServe(int servePlayer = 0)
    {
        if (servePlayer != 1 && servePlayer != 2) return;

        if (servePlayer == 1)
        {
            shutter.transform.position = shutterSpawnPoint1.position;
        }
        else if (servePlayer == 2)
        {
            shutter.transform.position = shutterSpawnPoint2.position;
        }

        SetWorldUI(serveUIGO, serverHolders[servePlayer - 1]);
        SetWorldUI(_serveMarkerGO, serverMarkerHolders[servePlayer - 1]);
    }

    private void ShutterHit()
    {
        serveUIGO.SetActive(false);
        _serveMarkerGO.SetActive(false);
    }

    private void SetWorldUI(GameObject UIGO, Transform newParent)
    {
        UIGO.SetActive(true);
        RectTransform rect = UIGO.GetComponent<RectTransform>();
        rect.SetParent(newParent, false);
        rect.localPosition = Vector3.zero;
        rect.localRotation = Quaternion.identity;
    }
}
