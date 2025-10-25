using UnityEngine;

public class CheckCourt : MonoBehaviour
{

    [SerializeField] private bool redCourt;
    [SerializeField] BadmintionGameManager _gameManager;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Shutter"))
        {
            if(redCourt)
            {
                Debug.Log("In Red Court");
                _gameManager.InRedCourt = true;
                //the ball is not in red court
            }
            else
            {
                Debug.Log("In Green Court");
                _gameManager.InRedCourt = false;
                //the ball is not in blue court
            }
        }
    }
}
