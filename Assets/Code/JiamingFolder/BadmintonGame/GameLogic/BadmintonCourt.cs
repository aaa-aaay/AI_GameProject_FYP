using UnityEngine;

public class BadmintonCourt : MonoBehaviour
{
    [SerializeField]private bool _isOpponentSide = false;
    [SerializeField] BadmintionGameManager _gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shutter"))
        {
            
            foreach(var shot in other.gameObject.GetComponents<Shot>())
            {
                shot.SetLocationMarker(Vector3.zero, true);
            }

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
