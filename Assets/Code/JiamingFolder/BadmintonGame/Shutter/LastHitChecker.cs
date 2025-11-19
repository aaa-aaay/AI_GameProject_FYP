using System;
using UnityEngine;

public class LastHitChecker : MonoBehaviour
{
    private GameObject racketThatLastHit;
    public event Action<GameObject> OnHitByRacker;
    [SerializeField] private BadmintionGameManager gameManager;

    private void Start()
    {
        racketThatLastHit = null;
        gameManager.OnPlayer1Score += ResetLastHit;
        gameManager.OnPlayer2Score += ResetLastHit;
    }


    private void OnDestroy()
    {
        gameManager.OnPlayer1Score -= ResetLastHit;
        gameManager.OnPlayer2Score -= ResetLastHit;
    }
    public void SetLastHitRacket(GameObject racket)
    {
        racketThatLastHit = racket;
        OnHitByRacker?.Invoke(racket);
        //if hit by gameobject, Ai should start going to center
        // if hit by ML Ai, my AI should start moving to the predicted location and try to hit back.

    }
    public GameObject GetLastHitRacker() { 
        return racketThatLastHit;
    }

    private void ResetLastHit()
    {
        racketThatLastHit = null;
    }
}
