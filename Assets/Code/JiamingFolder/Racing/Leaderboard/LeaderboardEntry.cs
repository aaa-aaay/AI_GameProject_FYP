using TMPro;
using UnityEngine;

public class LeaderboardEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text displayName;
    [SerializeField] private TMP_Text displayTimer;
    [SerializeField] private TMP_Text displayTimer2;
    [SerializeField] private TMP_Text displayTimer3;


    public void SetEntryData(string displayName, float timer)
    {
        int minutes = (int)(timer / 60f);
        int seconds = (int)(timer % 60f);
        int milliseconds = (int)((timer - (int)timer) * 1000f);

        this.displayName.text = displayName;


        // mm
        this.displayTimer.text = minutes.ToString("00");

        // ss
        this.displayTimer2.text = seconds.ToString("00");

        // ms
        this.displayTimer3.text = milliseconds.ToString("000");
    }

}
