using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class TutorialUIPlayer : MonoBehaviour
{
    private string _gameName;
    private List<VideoClip> _clips;

    private MiniGameSO _minigameSO;

    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private TMP_Text _gameTitleText;

    [SerializeField] private GameObject _nextButton;
    [SerializeField] private GameObject _previousButton;
    [SerializeField] private GameObject _goGameButton;


    int _currentClipIndex = 0;

    private void Start()
    {
        UIManager uiManager = ServiceLocator.Instance.GetService<UIManager>();

        _minigameSO = uiManager.GetMiniGameForTutorial();
        _clips = _minigameSO.tutorialVideoClips;
        _gameTitleText.text = _minigameSO.gameName;

        if (_clips.Count <= 0){
            GoGameScene();
            return;
        } 

        _currentClipIndex = 0;
        _videoPlayer.clip = _clips[_currentClipIndex];


        _previousButton.SetActive(false);
        _goGameButton.SetActive(false);

        if (_clips.Count == 1) {
            _goGameButton.SetActive(true);
            _nextButton.SetActive(false);
        } 
          

    }

    public void switchClips(bool next)
    {
        if (next)
        {
            _currentClipIndex++;
            _previousButton.SetActive(true);

            if (_currentClipIndex == _clips.Count - 1)
            {
                _nextButton.SetActive(false);
                _goGameButton.SetActive(true);
                _currentClipIndex = _clips.Count - 1;
            }   

        }
        else
        {
            _currentClipIndex--;
            _nextButton.SetActive(true);

            if (_currentClipIndex <= 0) 
            {
                _previousButton.SetActive(false);
                _currentClipIndex = 0;
            } 

        }

        _videoPlayer.clip = _clips[_currentClipIndex];
    }

    public void GoGameScene()
    {
        ServiceLocator.Instance.GetService<MySceneManager>().LoadScene(_minigameSO.sceneName);
    }


    
}
