using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RabbitUI : MonoBehaviour
{
    [Header("Runner UI Settings")]
    public List<Image> runnerIcons;       // List of runner icons in order
    public Sprite notCaughtSprite;        // Default image (runner not yet caught)
    public Sprite caughtSprite;           // Image when runner is caught

    private int currentCaughtCount = 0;

    void Start()
    {
        ResetUI();
    }

    
    public void OnRunnerCaptured()
    {
        if (runnerIcons == null || runnerIcons.Count == 0)
        {
            Debug.LogWarning("RunnerCaptureUI: No runner icons assigned!");
            return;
        }

        if (currentCaughtCount < runnerIcons.Count)
        {
            runnerIcons[currentCaughtCount].sprite = caughtSprite;
            currentCaughtCount++;
        }
        else
        {
            Debug.Log("All runners already caught!");
        }
    }

    /// <summary>
    /// Resets all icons to the "not caught" state.
    /// </summary>
    public void ResetUI()
    {
        if (runnerIcons == null || runnerIcons.Count == 0) return;

        foreach (var icon in runnerIcons)
        {
            if (icon != null && notCaughtSprite != null)
                icon.sprite = notCaughtSprite;
        }

        currentCaughtCount = 0;
    }
}
