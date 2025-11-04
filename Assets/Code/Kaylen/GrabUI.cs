using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GrabUI : MonoBehaviour
{
    [Header("UI References")]
    public Image activeIcon;    // Active/ready icon
    public Image inactiveIcon;  // Inactive/cooldown icon (should be a filled Image, Fill Method: Radial 360)

    private bool isOnCooldown = false;

    void Start()
    {
        if (activeIcon != null) activeIcon.enabled = true;

        if (inactiveIcon != null)
        {
            inactiveIcon.enabled = false;
            inactiveIcon.fillAmount = 0f;
        }
    }

    public void StartCooldownUI(float duration)
    {
        if (!isOnCooldown)
            StartCoroutine(CooldownRoutine(duration));
    }

    private IEnumerator CooldownRoutine(float duration)
    {
        isOnCooldown = true;

        if (inactiveIcon != null)
        {
            inactiveIcon.enabled = true;
            inactiveIcon.fillAmount = 1f; // start fully filled
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            if (inactiveIcon != null)
                inactiveIcon.fillAmount = Mathf.Lerp(1f, 0f, t); // radial cooldown effect

            yield return null;
        }

        if (inactiveIcon != null)
        {
            inactiveIcon.fillAmount = 0f;
            inactiveIcon.enabled = false;
        }

        if (activeIcon != null) activeIcon.enabled = true;

        isOnCooldown = false;
    }
}
