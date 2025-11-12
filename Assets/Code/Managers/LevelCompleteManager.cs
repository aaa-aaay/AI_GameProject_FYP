using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelCompleteManager : MonoBehaviour
{
    [SerializeField] private TMP_Text[] _starConditionTexts; 
    [SerializeField] private TMP_Text _levelFailedText; 
    [SerializeField] private Animator[] _starAnimators;
    [SerializeField] private GameObject _levelCompleteCanvas;
    [SerializeField] private GameObject _levelFailedCanvas;

    [SerializeField] MiniGameSO _debugSO;
    private void Start()
    {
        ToggleLevelCompleteCanvas(false);
        ToggleLevelFailedCanvas(false);
    }
    public void ToggleLevelCompleteCanvas(bool open,int starCount = 0, MiniGameSO gameSo = null)
    {

        if (open)
        {
            if (_levelFailedCanvas.activeSelf) return;

            _levelCompleteCanvas.SetActive(true);
            playStarAnimations(starCount);

            if(gameSo != null)
            SetStarConditionTexts(gameSo.starConditions);
            else SetStarConditionTexts(_debugSO.starConditions);
        }
        else
        {
            _levelCompleteCanvas.SetActive(false);
            RemoveStars();

        }
           
    }

    public void ToggleLevelFailedCanvas(bool open, MiniGameSO gameSo = null)
    {
        if (!_levelCompleteCanvas.activeSelf) _levelFailedCanvas.SetActive(open);
        if (gameSo != null)
            _levelFailedText.text = gameSo.levelFailedText;
        else 
            _levelFailedText.text = _debugSO.levelFailedText;
    }

    private void playStarAnimations(int starCount)
    {
        Debug.Log("StarCount: " + starCount);
        int count = starCount;
        foreach (var animator in _starAnimators)
        {
            Debug.Log("AnimationPlayed");
            animator.SetTrigger("GetStar");
            starCount--;
            if (starCount == 0) break;
        }
    }

    private void RemoveStars()
    {
        foreach (var animator in _starAnimators)
        {
            animator.SetTrigger("ResetStar");
        }
    }

    private void SetStarConditionTexts(string[] conditionStrings)
    {
        int count = 0;
        foreach (var conditionText in _starConditionTexts)
        {
            if (count >= conditionStrings.Count()) return;

            conditionText.text = conditionStrings[count];
            count++;
        }
    }
}
