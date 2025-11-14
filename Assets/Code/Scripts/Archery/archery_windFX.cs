using UnityEngine;

public class archery_windFX : MonoBehaviour
{
    [SerializeField] private Transform windParticleObject;
    [SerializeField] private ParticleSystem windParticle;
    [SerializeField] private float maxParticleSpeed = 2.0f;

    private float maxWindSpeed;

    public void Initialize()
    {
        maxWindSpeed = archery_handler.instance.settings.maxWindSpeed;
    }

    public void UpdateWind(int direction, float speed = 1)
    {
        float percentage = Mathf.Clamp01(speed / maxWindSpeed);

        if (speed < Mathf.Epsilon)
        {
            windParticleObject.gameObject.SetActive(false);
        }

        if (direction == 1)
            windParticleObject.transform.rotation = Quaternion.AngleAxis(90.0f, new Vector3(0,0,1));
        else
            windParticleObject.transform.rotation = Quaternion.AngleAxis(-90.0f, new Vector3(0, 0, 1));

        windParticle.startSpeed = percentage * maxParticleSpeed;

        windParticleObject.gameObject.SetActive(true);
    }

}
