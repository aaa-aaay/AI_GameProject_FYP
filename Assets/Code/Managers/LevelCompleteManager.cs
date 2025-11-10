using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelCompleteManager : MonoBehaviour
{
    [SerializeField] private Animator[] _starAnimators;
    [SerializeField] private GameObject _levelCompleteCanvas;
    [SerializeField] private GameObject _levelFailedCanvas;


    private void Start()
    {
        ToggleLevelCompleteCanvas(false);
        ToggleLevelFailedCanvas(false);
    }
    public void ToggleLevelCompleteCanvas(bool open,int starCount = 0)
    {

        if(!open) RemoveStars();
        if (!_levelFailedCanvas.activeSelf) {

            _levelCompleteCanvas.SetActive(open);
            playStarAnimations(starCount);


        } 
    }

    public void ToggleLevelFailedCanvas(bool open)
    {
        if (!_levelCompleteCanvas.activeSelf) _levelFailedCanvas.SetActive(open);
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
}
