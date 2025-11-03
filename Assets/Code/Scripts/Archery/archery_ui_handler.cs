using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class archery_ui_handler : MonoBehaviour
{
    [SerializeField] private TMP_Text textbox;
    [SerializeField] private RectMask2D powerMask;
    [SerializeField] private float maxPowerMask;

    private float maxForce;
    private float minForce;
    private float force;

    private int maxPoint;
    private int playerPoint;
    private int agentPoint;

    public void init_value(float maxForce, float minForce, int maxPoint)
    {
        this.maxForce = maxForce;
        this.minForce = minForce;
        this.maxPoint = maxPoint;
    }

    public void set_value(float force)
    {
        this.force = force; 

        UpdateUI();
    }

    public void set_point(int playerPoint, int agentPoint)
    {
        this.playerPoint = playerPoint;
        this.agentPoint = agentPoint;

        UpdateUI();
    }

    private void UpdateUI()
    {
        float forcePercent = Mathf.Clamp01((force - minForce) / (maxForce - minForce));

        Vector4 padding = powerMask.padding;
        padding.z = maxPowerMask - (maxPowerMask * forcePercent);
        powerMask.padding = padding;
    }
}
