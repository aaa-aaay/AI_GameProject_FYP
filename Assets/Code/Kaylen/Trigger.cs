using UnityEngine;
using TMPro; // For TextMeshPro

public class Trigger : MonoBehaviour
{
    [TextArea]
    public string tutorialMessage; // Different message for each trigger
    public TextMeshProUGUI tutorialText; // Reference to UI text

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Runner"))
        {
            tutorialText.text = tutorialMessage;
            tutorialText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Runner"))
        {
            tutorialText.text = ""; // Clear when leaving
            tutorialText.gameObject.SetActive(false);
        }
    }
}
