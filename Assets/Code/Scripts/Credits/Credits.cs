using UnityEngine;
using System.Collections.Generic;

public class Credits : MonoBehaviour
{
    [SerializeField] private List<GameObject> hide_during_credits;
    [SerializeField] private List<GameObject> show_during_credits;

    [SerializeField] private Animator animator;
    private void Start()
    {
        ServiceLocator.Instance.GetService<InputManager>().OnClick += EscPressed;
    }
    public void StartCredits(bool start)
    {
        if (start)
        {
            ServiceLocator.Instance.GetService<UIManager>().DisableSettings(true);
            animator.Play("ScrollingCredits", -1, 0);
        }
        for (int i = 0; i < hide_during_credits.Count; i++) 
        {
            hide_during_credits[i].SetActive(!start);
        }
        for (int i = 0;i < show_during_credits.Count;i++)
        {
            show_during_credits[i].SetActive(start);
        }
    }

    private void EscPressed()
    {
        ServiceLocator.Instance.GetService<UIManager>().DisableSettings(false);
        StartCredits(false);
    }
}
