using TMPro;
using UnityEngine;

public class LeaderboardEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text displayName;
    [SerializeField] private TMP_Text displayTimer;


    public void SetEntryData(string displayName, float timer)
    {
        this.displayName.text = displayName;
        this.displayTimer.text = timer.ToString("F2") + "s";
    }

}
