using TMPro;
using UnityEngine;

public class PongScore : MonoBehaviour
{
    [SerializeField] private TMP_Text player_score;
    [SerializeField] private TMP_Text opponent_score;
    
    public void UpdateScores(int new_player_score,  int new_opponent_score)
    {
        player_score.text = new_player_score.ToString();
        opponent_score.text = new_opponent_score.ToString();
    }
}
