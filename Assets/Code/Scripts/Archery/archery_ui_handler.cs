using TMPro;
using UnityEngine;

public class archery_ui_handler : MonoBehaviour
{
    [SerializeField] private TMP_Text textbox;

    private float force;
    private float yaw;
    private float pitch;
    private float windDirection;
    private float windSpeed;

    public void set_value(float force, float yaw, float pitch, float windDirection, float windSpeed)
    {
        this.force = force;
        this.yaw = yaw;
        this.pitch = pitch;
        this.windDirection = windDirection;
        this.windSpeed = windSpeed;
        UpdateUI();
    }

    private void UpdateUI()
    {
        textbox.text = $"Force: {force}\n" +
            $"Yaw: {yaw}\n" +
            $"Pitch: {pitch}\n" +
            $"Wind Direction: {windDirection}\n" +
            $"Wind Speed: {windSpeed}";
    }
}
