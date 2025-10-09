using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSelect : MonoBehaviour
{
    [SerializeField]
    private string _nextSceneName;

    [SerializeField]
    private string _levelName;

    private InputManager _inputManager;
    private UIManager _uiManager;
    private bool playerInRange = false;
    private Collider _collider;


    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }
    private void Start()
    {

       
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _inputManager.onInteract += HandleInteract;

        _uiManager = ServiceLocator.Instance.GetService<UIManager>();



        playerInRange = false;
    }

    private void HandleInteract()
    {

        if(playerInRange)
        {
            ServiceLocator.Instance.GetService<MySceneManager>().LoadScene(_nextSceneName);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            playerInRange = true;
            _uiManager.OpenLevelSelectUI(_levelName, 1);

        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            _uiManager.HideLevelSelectUI();
           

        }
    }

    public void Activate(bool activate)
    {
        if (activate) {

            _collider.enabled = true;

        }
        else
        {
            _collider.enabled = false;
        }
    }
}
