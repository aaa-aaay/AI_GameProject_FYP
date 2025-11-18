using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSelect : MonoBehaviour
{
    [SerializeField] private string _scene;
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
    }

    private void OnDestroy()
    {
        _inputManager.onInteract -= HandleInteract;
    }

    private void HandleInteract()
    {

        if(_playerInRange)
        {
            if (_scene != string.Empty)
            {
                ServiceLocator.Instance.GetService<MySceneManager>().LoadScene(_scene);
                return;
            }

            ServiceLocator.Instance.GetService<MySceneManager>().LoadMiniGameWithTutorial(_miniGame);
            ServiceLocator.Instance.GetService<AudioManager>().PlaySFX("LevelSelectFinish");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ServiceLocator.Instance.GetService<AudioManager>().PlaySFX("LevelSelect",transform.position);
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
