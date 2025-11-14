using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSelect : MonoBehaviour
{

    [SerializeField] private MiniGameSO _miniGame;

    private InputManager _inputManager;
    private UIManager _uiManager;
    private bool _playerInRange = false;
    private Collider _collider;
    private int _starCount = 0;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }
    private void Start()
    {
        _inputManager = ServiceLocator.Instance.GetService<InputManager>();
        _inputManager.onInteract += HandleInteract;

        _uiManager = ServiceLocator.Instance.GetService<UIManager>();

        _playerInRange = false;
        _starCount = 0;
    }

    private void OnDestroy()
    {
        _inputManager.onInteract -= HandleInteract;
    }

    private void HandleInteract()
    {

        if(_playerInRange)
        {
            ServiceLocator.Instance.GetService<MySceneManager>().LoadMiniGameWithTutorial(_miniGame);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            _playerInRange = true;
            _uiManager.OpenLevelSelectUI(_miniGame, _starCount);

        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
            _uiManager.HideLevelSelectUI();
           

        }
    }
    public void SetStarCount(int count)
    {
        _starCount = count;
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
