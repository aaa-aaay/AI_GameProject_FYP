using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSelect : MonoBehaviour
{
    [SerializeField]
    private int _nextSceneNumber;

    [SerializeField]
    private GameObject popUpCanvas;

    private InputManager _inputManager;

    private bool playerInRange = false;

    private void Start()
    {

       
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _inputManager.onInteract += HandleInteract;
        Debug.Log("InputManager found: " + (_inputManager != null));

        playerInRange = false;
        popUpCanvas.SetActive(false);
    }

    private void HandleInteract()
    {
        Debug.Log("interacted");

        if(playerInRange)
        {
            SceneManager.LoadScene(_nextSceneNumber);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            //show pop up UI to ask player if they want to enter next scene
            playerInRange = true;
            popUpCanvas.SetActive(true);
            


        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            popUpCanvas.SetActive(false);
           

        }
    }
}
