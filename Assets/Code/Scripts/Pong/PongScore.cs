using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PongScore : MonoBehaviour
{
    [SerializeField] private Image player_score;
    [SerializeField] private Image opponent_score;

    [SerializeField] private List<NumberToImage> images;
    
    public void UpdateScores(int new_player_score,  int new_opponent_score)
    {
        player_score.sprite = GetSpriteByValue(new_player_score);
        opponent_score.sprite = GetSpriteByValue(new_opponent_score);
    }

    public Sprite GetSpriteByValue(int number)
    {
        for (int i = 0; i < images.Count; i++)
        {
            if (number == images[i].value)
            {
                return images[i].image;
            }
        }

        return images[0].image;
    }
}
