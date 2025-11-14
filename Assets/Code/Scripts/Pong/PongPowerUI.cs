using UnityEngine;

public class PongPowerUI : MonoBehaviour
{
    [SerializeField] private BuffType type;
    [SerializeField] private float destroy_time;

    private float time_passed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        time_passed = 0;

        PongUI.instance.CreateNew(gameObject, type);
    }

    // Update is called once per frame
    void Update()
    {
        time_passed += Time.deltaTime;

        if (time_passed > destroy_time)
        {
            time_passed = destroy_time;
            PongUI.instance.UpdateSlider(gameObject, 0);
            Destroy(gameObject);
        }
    }
}
