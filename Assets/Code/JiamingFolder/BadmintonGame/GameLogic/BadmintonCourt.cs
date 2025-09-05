using UnityEngine;

public class BadmintonCourt : MonoBehaviour
{
    [SerializeField]private bool _isOpponentSide = false;
    [SerializeField] BadmintionGameManager _gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shutter"))
        {
            if (!_isOpponentSide)
            {
               _gameManager.PlayerScores(2); //oppoent scores
            }
            else
            {
                _gameManager.PlayerScores(1);
            }




        }
    }
}
