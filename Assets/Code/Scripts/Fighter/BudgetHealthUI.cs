using TMPro;
using UnityEngine;

public class BudgetHealthUI : MonoBehaviour
{
    private TMP_Text text;
    private Damageable damageable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TMP_Text>();
        damageable = transform.root.gameObject.GetComponent<Damageable>();

        text.text = damageable.get_health().ToString();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        text.text = damageable.get_health().ToString();
        transform.forward = Camera.main.transform.forward;
    }
}
