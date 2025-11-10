using UnityEngine;
using UnityEngine.VFX;

public class OnScore : MonoBehaviour
{
    [SerializeField] private GameObject owner;
    private VisualEffect effect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        effect = GetComponent<VisualEffect>();
    }

    public void OnScorePlayVFX(GameObject player, GameObject ball)
    {
        if (ball == owner)
        {
            transform.position = ball.transform.position;
            effect.Play();
        }
    }
}
