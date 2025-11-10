using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.BoolParameter;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject _settingsPage;
    [SerializeField] private TMP_Text bgmVolText;
    [SerializeField] private TMP_Text sfxVolText;
    [SerializeField] private TMP_Text _screenTypeText;
    [SerializeField] private TMP_Text difficultyText;
    private AudioManager _audioManager;

    public bool _isSettingsOpen;


    private enum displayType
    {
        Fullscreen = 0,
        Windowed = 1,
        WindowedFullScreen = 2,
    }
    private displayType _currentDisplayType;
    private DifficultyLevel _currentDifficultySettings;
    private int maxDisplayType = 3;
    private void Start()
    {
        _audioManager = ServiceLocator.Instance.GetService<AudioManager>();
        SetDifficultyText();
        SetDisplayType(displayType.Fullscreen);
    }
    public void UpdateBGMVol(float newVolume)
    {
       
        if (_audioManager != null) _audioManager.SetBGMVol(newVolume);

            bgmVolText.text = Mathf.RoundToInt(newVolume * 100).ToString();
    }

    public void UpdateSFXVol(float newVolume)
    {
        if (_audioManager != null) _audioManager.SetSFXVol(newVolume);
        sfxVolText.text = Mathf.RoundToInt(newVolume * 100).ToString();
    }
    public void SwitchDisplayType(bool next)
    {
        int index = (int)_currentDisplayType;
        if (next)
        {
            if (index < maxDisplayType - 1) index++;
            else index = 0;
        }
        else
        {
            if(index != 0) index--;
            else index = maxDisplayType - 1;
        }
        _currentDisplayType = (displayType)index;

        SetDisplayType(_currentDisplayType);
    }
    private void SetDisplayType(displayType type)
    {
        switch (type)
        {
            case displayType.Fullscreen:
                Screen.SetResolution(1440, 1080, FullScreenMode.ExclusiveFullScreen);
                _screenTypeText.text = "FullScreen";
                break;

            case displayType.Windowed:
                Screen.SetResolution(1440, 1080, FullScreenMode.Windowed);
                _screenTypeText.text = "Windowed";
                break;

            case displayType.WindowedFullScreen:
                Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.FullScreenWindow);
                _screenTypeText.text = "WindowedFullscreen";
                break;
        }
    }

    public void ToggleSettings(bool open)
    {
        _settingsPage.SetActive(open);
        if (open) Time.timeScale = 0; else Time.timeScale = 1;
        _isSettingsOpen = open;
    }

    public void ChangeDifficulty(bool next)
    {
        int index = (int) _currentDifficultySettings;
        if (next)
        {
            if (index < (int) DifficultyLevel.Count - 1) index++;
            else index = 0;
        }
        else
        {
            if (index != (int) DifficultyLevel.Easy) index--;
            else index = index - 1;
        }
        PlayerPrefs.SetInt("Difficulty", index);
        _currentDifficultySettings = (DifficultyLevel) index;
        SetDifficultyText(_currentDifficultySettings);

    }

    private void SetDifficultyText()
    {
        SetDifficultyText((DifficultyLevel)PlayerPrefs.GetInt("Difficulty"));
    }

    private void SetDifficultyText(DifficultyLevel difficultySettings)
    {
        switch (difficultySettings)
        {
            case DifficultyLevel.Easy:
                difficultyText.text = "Easy";
                break;
            case DifficultyLevel.Medium:
                difficultyText.text = "Medium";
                break;
            case DifficultyLevel.Hard:
                difficultyText.text = "Hard";
                break;
            default:
                break;
        }
    }
}
